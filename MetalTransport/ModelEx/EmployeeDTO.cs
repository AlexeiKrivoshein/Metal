using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class EmployeeDTO 
        : BaseDTO, INotifyPropertyChanged
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _secondname = "";
        public string Secondname
        {
            get => _secondname;
            set
            {
                _secondname = value;
                OnPropertyChanged("Secondname");
            }
        }

        private string _patronymic = "";
        public string Patronymic
        {
            get => _patronymic;
            set
            {
                _patronymic = value;
                OnPropertyChanged("Patronymic");
            }
        }

        private Nullable<Guid> _postId = null;
        public Nullable<Guid> PostId
        {
            get => _postId;
            set
            {
                _postId = value;
                OnPropertyChanged("PostId");
            }
        }

        private bool _useForLogin = false;
        public bool UseForLogin
        {
            get => _useForLogin;
            set
            {
                _useForLogin = value;
                OnPropertyChanged("UseForLogin");
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        private Nullable<Guid> _userGroupId;
        public Nullable<Guid> UserGroupId
        {
            get => _userGroupId;
            set
            {
                _userGroupId = value;
                OnPropertyChanged("UserGroupId");
            }
        }

        private bool _deleted = false;
        public bool Deleted
        {
            get => _deleted;
            set
            {
                _deleted = value;
                OnPropertyChanged("Deleted");
            }
        }

        private long _version;
        public long Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        private PostDTO _post;
        public PostDTO Post
        {
            get => _post;
            set
            {
                _post = value;
                OnPropertyChanged(nameof(Post));
            }
        }

        private UserGroupDTO _userGroup;
        public UserGroupDTO UserGroup
        {
            get => _userGroup;
            set
            {
                _userGroup = value;
                OnPropertyChanged(nameof(UserGroup));
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is EmployeeDTO other))
                return false;

            return  this.Id == other.Id &&
                    this.Name == other.Name &&
                    this.Secondname == other.Secondname &&
                    this.Patronymic == other.Patronymic &&
                    this.PostId == other.PostId &&
                    this.UseForLogin == other.UseForLogin &&
                    this.Password == other.Password &&
                    this.Deleted == other.Deleted &&
                    this.UserGroupId == other.UserGroupId;
        }
    }
}
