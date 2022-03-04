using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Security;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Linq;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MetalClient.ViewModel
{
    public class LoginViewModel
        : SimpleElementsListViewModel<VersionListItemDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.UserLogin;

        protected override DatagramType RemoveDatagramType => DatagramType.NONE;

        private SecurityContext _context = null;
        public SecurityContext Context
        {
            get => _context;
            set
            {
                _context = value;
                OnPropertyChanged(nameof(Context));
            }
        }

        private RelayCommand _loginCommand;
        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ??
                  (_loginCommand = new RelayCommand(obj =>
                  {
                      var token = PrgShow?.Invoke("Вход пользователя").Token ?? CancellationToken.None;
                      InnerLogin(obj, token).ContinueWith((t) => PrgHide?.Invoke());
                  }));
            }
        }

        public LoginViewModel(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
            : base(id, dataManager, selectType) { }

        protected override Task<List<VersionListItemDTO>> InnerLoad(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetUsersList).
                WithDTOObject(new GetAllVersionElementsList(false, -1)).
                Build();

            return DataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, token)
                .ContinueWith((t) =>
                {
                    if (!TaskHelper.CheckError(t, out var error))
                    {
                        ErrorInvoke(error);
                        return null;
                    }

                    var data = t.Result;
                    return data.Elements;
                });
        }

        private Task InnerLogin(object password, CancellationToken token)
        {
            if (!(password is PasswordBox passwordBox))
            {
                throw new ArgumentException("Объект доступа к паролю имеет не верный тип");
            }

            var factory = new DatagramFactory();
            DatagramBase request = null;

            using (MD5 md5Hash = MD5.Create())
            {
                request = factory.WithType(DatagramType.UserLogin)
                                 .WithDTOObject(new GetLoginElement(Selected.Id, SecurityHelper.GetMd5Hash(md5Hash, passwordBox.Password)))
                                 .Build();
            }

            return DataManager.ExcuteRequestAsync<SecurityContext>(request, token).
                ContinueWith((t1) =>
                {
                    var result = t1.Result;

                    if (result.UserId == Selected.Id &&
                            Selected.Id != Guid.Empty)
                    {
                        Context = result;

                        File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\user.def", Selected.Id.ToString());
                        IsSelected = true;
                        SelectedInvoke();
                    }
                    else
                    {
                        ErrorInvoke("Не удалось выполнить вход пользователя");
                    }
                }, token);
        }
    }
}
