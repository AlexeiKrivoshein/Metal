using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using MetalTransport.ModelEx.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MetalClient.Helper
{
    internal static class MessageBoxHelper
    {
        public static bool CheckAndShowSaveError(string name, Task<HandledDTO> task, Window owner = null)
        {
            if (!TaskHelper.CheckError(task, out var error))
            {
                MetalMessageBox.Show("Ошибка", GetErrorString("сохранить", name, error), MessageBoxImage.Error, MessageBoxButton.OK, owner);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckAndShowRemoveError(string name, Task<HandledDTO> task, Window owner = null)
        {
            if (!TaskHelper.CheckError(task, out var error))
            {
                MetalMessageBox.Show("Ошибка", GetErrorString("удалить", name, error), MessageBoxImage.Error, MessageBoxButton.OK, owner);
                return false;
            }
            else
            {
                return true;
            }
        }

        private static string GetErrorString(string name, string action, string error)
        {
            return $"Не удалось {action} {name}: {error}.";
        }

        public static void ErrorMesage(string header, string message, Window owner = null)
        {
            MetalMessageBox.Show(string.IsNullOrWhiteSpace(header) ? "Ошибка" : header, $"{message}.", MessageBoxImage.Error, MessageBoxButton.OK, owner);
        }

        public static void InformationMesage(string header, string message, Window owner = null)
        {
            MetalMessageBox.Show(string.IsNullOrWhiteSpace(header) ? "Информация" : header, $"{message}.", MessageBoxImage.Information, MessageBoxButton.OK, owner);
        }
    }
}
