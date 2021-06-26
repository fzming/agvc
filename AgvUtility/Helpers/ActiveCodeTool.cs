using System;
using System.Threading;

namespace Utility.Helpers
{
   /// <summary>
   /// 激活码生成器
   /// </summary>
    public  static class ActiveCodeTool
    {
        private const int Mapkeylen = 16;
        private const int Registkeylen = 25;
        private static readonly string[] Mapketitem = { "U", "F", "2", "I", "X", "S", "A", "V", "P", "M", "C", "9", "Q", "1", "G", "7", "K", "T", "B", "4", "W", "6", "H", "N", "E", "8", "Z", "O", "L", "3", "D", "0", "Y", "5", "J", "R" };

        private static readonly int[] MapAreas = { 48, 60, 74, 90 };
        private static readonly int[] RegAreas = { 48, 64, 88, 100, 116 };
     
        /// <summary>
        /// 获取键
        /// </summary>
        /// <param name="keyType">1是4组4位码，2是5组5位码。可以通通过上面的值定义</param>
        /// <returns></returns>
        public static string GetKey(int keyType)
        {
            var mapKeySplit = "-";
            var mapKeyA = GetKeys(0, keyType);
            var mapKeyB = GetKeys(1, keyType);
            var mapKeyC = GetKeys(2, keyType);
            var mapKeyD = GetKeys(3, keyType);
            var mapKey = mapKeyA + mapKeySplit + mapKeyB + mapKeySplit + mapKeyC + mapKeySplit + mapKeyD;
            if (keyType != 2) return mapKey;
            var mapKeyE = GetKeys(4, keyType);
            mapKey += mapKeySplit + mapKeyE;
            return mapKey;
        }

        private static string GetKeys(int index, int keyType = 1)
        {
            var item = string.Empty;//当前组的值
            var areas = MapAreas; //合计域
            var keyItemsLen = Mapkeylen / 4;//每组长度
            if (keyType == 2)
            {
                areas = RegAreas;
                keyItemsLen = Registkeylen / 5;
            }
            if (index >= 0 && index < keyItemsLen)
            {
                var preItem = 0;//前一个值
                var total = 0;//当前合计
                for (var i = 0; i < keyItemsLen; i++)
                {
                    var curr = GetRight(areas[index], preItem, i, keyItemsLen, total);
                    total += curr;
                    if (curr >= Mapketitem.Length || curr <= 0 || (total >= areas[index] && i < keyItemsLen - 2))
                    {
                        item = GetKeys(index, keyType);
                    }
                    else
                    {
                        preItem = curr;
                        item += Mapketitem[curr];
                    }
                }
            }
            else
            {
                item = "0000";
            }
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area">总和的值</param>
        /// <param name="preItem">前一个的值</param>
        /// <param name="no">当前到第几个数上了</param>
        /// <param name="keyItemsLen"></param>
        /// <param name="total">当前总计</param>
        /// <returns></returns>
        private static int GetRight(int area, int preItem, int no, int keyItemsLen, int total)
        {
           
            int item;
            var len = Mapketitem.Length;
            if (no == keyItemsLen - 1)//当获取到最后一个数时，直接总和值减去当前总和
            {
                item = area - total;
            }
            else
            {
                var random = new Random();
                item = len - 1 > area - total ? random.Next(1, area - total) : random.Next(1, len - 1);
                Thread.Sleep(50);
            }
            //极端情况处理
            //a!=area-keyItemsLen+1,因为第一个值取太大使得后三个值都是1，但这种情况在此题中不存在
            //b!=area-total-keyItemsLen+1，因为前两个值的和值太大使得后两个值都是1，这种情况下重新取第二个数
            //c!=area-total-c，虽然最后两个值不是1，但是也可能使得最后两个值相等，当知道这个值会和最后一个值相等时，抛弃此值重新取一个值
            if (item == preItem || no == 0 && item == area - keyItemsLen + 1 ||
                (no == 1 && item == area - total - keyItemsLen + no + 1) ||
                (no == 3 && item == area - total - item)) //item<=0时重新生成所有，故不在此判断
            {
                item = GetRight(area, preItem, no, keyItemsLen, total);
            }
            return item;
        }
    }

}