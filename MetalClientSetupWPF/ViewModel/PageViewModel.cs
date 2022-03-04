using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MetalClientSetupWPF.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;

namespace MetalClientSetupWPF.ViewModel
{
    public abstract class PageViewModel
        : ViewModelBase
    {
        private IVariableSource _variableSource;

        private string _shortName;
        public string ShortName
        {
            get => _shortName;
            set => Set(nameof(ShortName), ref _shortName, value);
        }

        private string _header;
        public string Header
        {
            get => _header;
            set => Set(nameof(Header), ref _header, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => Set(nameof(Description), ref _description, value);
        }

        private List<CommandViewModel> _commands;
        public List<CommandViewModel> Commands
        {
            get => _commands;
            set => Set(nameof(Commands), ref _commands, value);
        }

        protected virtual bool OnNext() => true;

        protected virtual bool OnPrev() => true;

        protected virtual bool OnCancel() => true;

        protected void ModifyVariables() => _variableSource.Modify();

        private bool _nextCommandEnabled;
        public bool NextCommandEnabled
        {
            get => _nextCommandEnabled;
            set => Set("NextCommandEnabled", ref _nextCommandEnabled, value);
        }

        private bool _prevCommandEnabled;
        public bool PrevCommandEnabled
        {
            get => _prevCommandEnabled;
            set => Set(nameof(PrevCommandEnabled), ref _prevCommandEnabled, value);
        }

        private bool _cancelCommandEnabled;
        public bool CancelCommandEnabled
        {
            get => _cancelCommandEnabled;
            set => Set(nameof(CancelCommandEnabled), ref _cancelCommandEnabled, value);
        }

        private bool? _success;
        public bool? Success
        {
            get => _success;
            set => Set("Success", ref _success, value);
        }

        private bool _current;
        public bool Current
        {
            get => _current;
            set => Set(nameof(Current), ref _current, value);
        }

        public bool IsInstalled => _variableSource.IsInstalled;

        protected PageViewModel(string shortName, string header, string description, IVariableSource variableSource)
        {
            _variableSource = variableSource;

            ShortName = shortName;
            Header = header;
            Description = description;

            NextCommandEnabled = true;
            PrevCommandEnabled = true;
            CancelCommandEnabled = true;
        }

        protected CommandViewModel Install()
        {
            return new CommandViewModel()
            {
                DisplayName = "УСТАНОВИТЬ",
                Command = new RelayCommand(() =>
                {
                    if (OnNext())
                    {
                        Messenger.Default.Send(new NextMessage()
                        {
                            Page = this
                        });
                    }
                }, () => NextCommandEnabled, true)
            };
        }

        protected CommandViewModel Modify()
        {
            return new CommandViewModel
            {
                DisplayName = "ИЗМЕНИТЬ",
                Command = new RelayCommand(() =>
                {
                    if (OnNext())
                    {
                        Messenger.Default.Send(new NextMessage
                        {
                            Page = this,
                        });
                    }
                }, () => NextCommandEnabled, true)
            };
        }

        protected CommandViewModel Uninstall()
        {
            return new CommandViewModel
            {
                DisplayName = "УДАЛИТЬ",
                Command = new RelayCommand(() =>
                {
                    if (OnNext())
                    {
                        if (MessageBox.Show($"Вы действительно хотите удалить Metal client?", $"Удаление Metal client",
                            MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.OK)
                        {
                            if (OnCancel())
                            {
                                Messenger.Default.Send(new UninstallMessage
                                {
                                    Page = this,
                                });
                            }
                        }
                    }
                }, () => NextCommandEnabled, true)
            };
        }

        protected CommandViewModel Next()
        {
            return new CommandViewModel()
            {
                DisplayName = "ДАЛЕЕ",
                Command = new RelayCommand(() =>
                {
                    if (OnNext())
                    {
                        Messenger.Default.Send(new NextMessage()
                        {
                            Page = this
                        });
                    }
                }, () => NextCommandEnabled, true)
            };
        }

        protected CommandViewModel NextVersion(Version coreVersion)
        {
            return new CommandViewModel()
            {
                DisplayName = "ДАЛЕЕ",
                Command = new RelayCommand(() =>
                {
                    if (OnNext())
                    {
                        if (_variableSource.InstalledVersion > coreVersion)
                        {
                            Messenger.Default.Send(new ErrorMessage
                            {
                                Error = $"На компьютере обнаружена более новая версия продукта {_variableSource.InstalledVersion}. " +
                                        $"Версия инсталлятора: {coreVersion}. Понижение версии запрещено."
                            });
                        }
                        else
                        {
                            Messenger.Default.Send(new NextMessage()
                            {
                                Page = this
                            });
                        }
                    }
                }, () => NextCommandEnabled, true)
            };
        }

        protected CommandViewModel Cancel()
        {
            return new CommandViewModel()
            {
                DisplayName = "ОТМЕНА",
                Command = new RelayCommand(() =>
                {
                    if (MessageBox.Show($"Вы действительно хотите отменить установку Metal client?", $"Установка Metal client",
                        MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.OK)
                    {
                        if (OnCancel())
                        {
                            Messenger.Default.Send(new CancelMessage());
                        }
                    }
                },
                () => CancelCommandEnabled, true)
            };
        }

        protected CommandViewModel CancelInstall()
        {
            return new CommandViewModel()
            {
                DisplayName = "ОТМЕНА",
                Command = new RelayCommand(() =>
                {
                    if (MessageBox.Show($"Вы действительно хотите отменить установку Metal client?", $"Установка Metal client",
                        MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.OK)
                    {
                        if (OnCancel())
                        {
                            Messenger.Default.Send(new CancelInstallMessage());
                        }
                    }
                }, true)
            };
        }

        protected CommandViewModel CancelUninstall()
        {
            return new CommandViewModel()
            {
                DisplayName = "ОТМЕНА",
                Command = new RelayCommand(() =>
                {
                    if (MessageBox.Show($"Вы действительно хотите отменить удаление Metal client?", $"Удаление Metal client",
                        MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.OK)
                    {
                        if (OnCancel())
                        {
                            Messenger.Default.Send(new CancelUninstallMessage());
                        }
                    }
                }, true)
            };
        }

        protected CommandViewModel Ready()
        {
            return new CommandViewModel()
            {
                DisplayName = "ГОТОВО",
                Command = new RelayCommand(() =>
                {
                    if (OnCancel())
                    {
                        Messenger.Default.Send(new CloseAppMessage());
                    }
                }, true)
            };
        }

        protected CommandViewModel Prev()
        {
            return new CommandViewModel()
            {
                DisplayName = "НАЗАД",
                Command = new RelayCommand(() =>
                {
                    if (OnPrev())
                    {
                        Messenger.Default.Send(new PrevMessage()
                        {
                            Page = this
                        });
                    }
                }, () => PrevCommandEnabled, true)
            };
        }

        public void InitCommands()
        {
            Commands = new List<CommandViewModel>(GetViewCommands());
        }

        protected abstract IEnumerable<CommandViewModel> GetViewCommands();

        public virtual void Activate() { }

        protected void SaveVariables() => _variableSource.Save();

        protected string GetVariable(string variableName) =>
            _variableSource.GetVariable(variableName);

        protected void SetVariable(string variableName, string value)
        {
            _variableSource.SetVariable(variableName, value);
        }

        protected void SetVariable<T>(Expression<Func<T>> propertyExpression, string variableName, string value)
        {
            _variableSource.SetVariable(variableName, value);
            RaisePropertyChanged(propertyExpression);
        }

        protected void SetVariable<T>(Expression<Func<T>> propertyExpression, string variableName, string value, params Action[] onChanged)
        {
            _variableSource.SetVariable(variableName, value);
            Array.ForEach(onChanged, a => a.Invoke());
            RaisePropertyChanged(propertyExpression);
        }
    }
}
