using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalClient.Helper
{
    public class UserGroupHelper
    {
        //основная группа полей, доступна для редактирования до сохранения заказа
        public const int NOT_SAVED = -1;
        // заказ
        public const int MAIN_1 = 0;
        public const int TECH_1_2 = 1;
        public const int DIRECTOR_3 = 2;
        public const int SALES_4_5 = 3;
        public const int ACC_8_10 = 4;
        public const int SALES_13 = 5;
        public const int DIRECTOR_14 = 6;
        public const int MAN_15_18 = 7;
        public const int OTK_19_22 = 8;
        public const int MAN_23_24 = 9;
        public const int ACC_25_27 = 10;
        public const int SEND_28_29 = 11;
        // пользователи
        public const int USERS_GROUP = 12;
        //материалы

        //!!! последний элемент необходимо следить что индекс равен индексу полденего права +1
        public const int LAST = 13;

        public static string GetRightName(int index)
        {
            switch (index)
            {
                case MAIN_1:
                    return "Основные реквизиты, гр. 1";
                case TECH_1_2:
                    return "Тех.отдел, гр. 1-2";
                case DIRECTOR_3:
                    return "Директор произв., гр. 3";
                case SALES_4_5:
                    return "Коммер.отдел, гр. 4-7";
                case ACC_8_10:
                    return "Бухгалтерия, гр. 8-10";
                case SALES_13:
                    return "Коммер.Отдел.Снабжение, гр. 13";
                case DIRECTOR_14:
                    return "Директор произв., гр. 14";
                case MAN_15_18:
                    return "Произв.участок, гр. 15-18";
                case OTK_19_22:
                    return "Начальник ОТК, гр. 19-22";
                case MAN_23_24:
                    return "Произв.участок, гр. 23-24";
                case ACC_25_27:
                    return "Бухгалтерия, гр. 25-27";
                case SEND_28_29:
                    return "Отгрузка, гр. 28-29";
                case USERS_GROUP:
                    return "Группы пользователей";
                default:
                    return "";
            }
        }

        public static bool IsEditing(object value, object parameter)
        {
            if (value is Locker locker &&
                parameter is int position)
            {

                if(locker.NotSaved)
                {
                    return position == NOT_SAVED;
                }
                else
                {
                    if (position == NOT_SAVED)
                    {
                        position = MAIN_1;
                    }
                }

                if (locker.StateLock)
                    return false;

                if (locker.UserLock)
                    return false;

                if (locker.Rights[position] > 1)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public static bool IsVisible(Locker locker, int from, int to)
        {
            for (int index = from; index <= to; index++)
                if (locker.Rights[index] > 0)
                    return true;

            return false;
        }
    }
}
