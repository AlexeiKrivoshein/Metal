using MetalTransport.ModelEx.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public abstract class BaseDTO
        : INotifyPropertyChanged, IDataErrorInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        //Словарь полей с ошибкой валидации данных [имя поля, текст ошибки]
        [NonSerialized]
        protected Dictionary<string, string> _invalidProperties = new Dictionary<string, string>();

        public BaseDTO(){}

        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return true;

            if (!(obj is BaseDTO typed)) return false;

            if (Id != typed.Id) return false;

            return typed.InnerEquals(this);
        }

        protected virtual bool InnerEquals(BaseDTO obj) => true;

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseDTO element1, BaseDTO element2)
        {
            return Equals(element1, element2);
        }

        public static bool operator !=(BaseDTO element1, BaseDTO element2)
        {
            return !(element1 == element2);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Валидация
        protected static ConcurrentDictionary<string, ValidationMapTuple> validationMap = new ConcurrentDictionary<string, ValidationMapTuple>();

        protected static void InitValidation(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (property is null)
                {
                    continue;
                }

                var attributes = property.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is ValidationBaseAttribute validationBaseAttribute)
                    {
                        validationMap.AddOrUpdate(property.Name, new ValidationMapTuple
                        {
                            Property = property,
                            ValidationAttributes = new List<ValidationBaseAttribute>() { validationBaseAttribute }
                        }, (k, v) => {
                            v.ValidationAttributes.Add(validationBaseAttribute);
                            return v;
                        });
                    }
                }
            }
        }

        public string Error => null;

        public string this[string propertyName]
        {
            get
            {
                string error = string.Empty;

                if (validationMap.ContainsKey(propertyName))
                {
                    var validationTuple = validationMap[propertyName];
                    if (Validator.Validate(validationTuple, this, out error))
                    {
                        RemoveInvalidProperty(propertyName);
                    }
                    else
                    {
                        AddInvalidProperty(propertyName, error);
                    }
                }

                return error;
            }
        }

        protected void AddInvalidProperty(string propertyName, string error)
        {
            _invalidProperties[propertyName] = error;
            IsValid = false;
        }

        protected void RemoveInvalidProperty(string propertyName)
        {
            if (_invalidProperties.ContainsKey(propertyName))
            {
                _invalidProperties.Remove(propertyName);
            }

            if (!_isValid && !_invalidProperties.Any())
            {
                IsValid = true;
            }
        }

        public event Action<bool> OnValidChange;

        private bool _isValid = true;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnValidChange?.Invoke(_isValid);
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }
        #endregion
    }
}
