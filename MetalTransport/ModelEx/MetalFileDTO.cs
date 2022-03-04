using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class MetalFileDTO
        : BaseDTO
    {
        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged("Index");
            }
        }

        private int _chunkCount;
        public int ChunkCount
        {
            get => _chunkCount;
            set
            {
                _chunkCount = value;
                OnPropertyChanged("ChunkCount");
            }
        }

        private Guid _orderId = Guid.Empty;
        public Guid OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OnPropertyChanged("OrderId");
            }
        }

        private byte[] _data;
        public byte[] Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is MetalFileDTO other))
                return false;

            return Id == other.Id;
        }
    }
}
