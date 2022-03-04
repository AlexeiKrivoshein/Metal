using MetalClient.DataManager;
using MetalClient.Helper;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class UserGroupViewModel
        : ElementViewModelBase<UserGroupDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.SetUserGroupElement;

        protected override DatagramType GetDatagramType => DatagramType.GetUserGroupElement;

        //колллекция для визуализации прав
        public ObservableCollection<Right> Rights { get; set; } = new ObservableCollection<Right>();

        private RelayCommand _setRightsCommand;
        public RelayCommand SetRightsCommand
        {
            get
            {
                return _setRightsCommand ??
                  (_setRightsCommand = new RelayCommand(obj =>
                  {
                      if (obj is bool state)
                      {
                          SetRights(state);
                      }
                  }));
            }
        }

        public UserGroupViewModel(Guid id, ClientDataManager dataManager, ElementState state)
            : base(id, dataManager, state) 
        {
        }

        private void SetRights(bool state)
        {
            foreach (var right in Rights)
            {
                right.Value = state ? (byte)2 : (byte)0;
            }
        }

        protected override Task<bool> AfterLoad(CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                return PrepareRightsView();
            });
        }

        public bool PrepareRightsView()
        {
            _dispatcher.Invoke(
                new Action(() =>
                {
                    Rights.Clear();

                    Rights.Add(new Right("Заказы", 0, true));
                    for (int index = 0; index <= UserGroupHelper.SEND_28_29; index++)
                    {
                        if (!string.IsNullOrEmpty(UserGroupHelper.GetRightName(index)))
                            Rights.Add(new Right(UserGroupHelper.GetRightName(index), Element.Rights[index], false));
                    }

                    Rights.Add(new Right("Группы пользователей", 0, true));
                    {
                        for (int index = UserGroupHelper.USERS_GROUP; index <= UserGroupHelper.USERS_GROUP; index++)
                            if (!string.IsNullOrEmpty(UserGroupHelper.GetRightName(index)))
                                Rights.Add(new Right(UserGroupHelper.GetRightName(index), Element.Rights[index], false));
                    }
                }));

            return true;
        }

        protected override Task BeforeSave(CancellationToken token, CancellationTokenSource cts)
        {
            return Task.Factory.StartNew(() =>
            {
                for (int index = 0; index < Element.Rights.Length; index++)
                    Element.Rights[index] = 0;

                var insertIndex = 0;
                for (int index = 0; index < Rights.Count; index++)
                {
                    if (!Rights[index].IsHeader)
                    {
                        Element.Rights[insertIndex] = Rights[index].Value;
                        insertIndex++;
                    }
                }
            });
        }
    }
}
