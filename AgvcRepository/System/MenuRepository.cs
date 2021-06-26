using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.System;
using AgvcRepository.System.Interfaces;
using CoreRepository;
using MongoDB.Driver;

namespace AgvcRepository.System
{
    /// <summary>
    ///     菜单仓储实现
    /// </summary>
    public class MenuRepository : MongoRepository<Menu>, IMenuRepository
    {
        public MenuRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<bool> BatchUpdateMenusMetaAsync(List<MenuRoleUpdateSet> roleUpdateSets)
        {
            var bulkModelList = roleUpdateSets.Select(CreateMenuUpdateOneModel).ToList();

            var rs = await Collection.BulkWriteAsync(bulkModelList);
            return rs.ModifiedCount > 0;
        }

        public async Task<IEnumerable<Menu>> QueryModuleMenusAsync(ModuleType[] modules)
        {
            var query = Collection.FindAsync(Filter.Or(Filter.AnyIn(p => p.Modules, modules),
                Filter.Exists(p => p.Modules, false)));
            return (await query).ToEnumerable();
        }

        private UpdateOneModel<Menu> CreateMenuUpdateOneModel(MenuRoleUpdateSet menuRoleUpdate)
        {
            /*
             * 对数组的操作：
             *1.Push 已有的数组末尾加入一个元素，要是元素不存在，就会创建一个新的元素
             *    + PushEach修饰符一起添加多个值到数组字段中
             *2.AddToSet 往数组里面加入数据，如果数组里已经存在，则不会加入（避免重复） 
             *      AddToSetEach 修饰符一起添加多个值到数组字段中
             *3.Pull 删除数组单个元素，将匹配的元素删除
             *      PullAll 删除数组多个元素，将所有匹配的元素删除
             *      PullFilter 根据指定的条件删除数组匹配元素
             *      PopFirst 从数组头部删除1个元素
             *      PopLast  从数组末尾删除1个元素
             * 4.Set 修改数组中的元素值
             *   Update.Set("Meta.Roles.1",value) //修改 Roles数组的元素位置1
             *   Update.Set("Meta.Roles.$",value) //修改位置0元素 等价于：Update.Set("Meta.Roles.0",value)
             *   Update.Set("array.$.field",value) //修改元素的属性值
             *   $[] 操作符 https://docs.mongodb.com/manual/reference/operator/update/positional-filtered/#up._S_[%3Cidentifier%3E] 
             *   Update.Set("Meta.Roles.$[]",value) //批量修改数组所有元素的值为value
             *   Update.Set("Meta.Roles.$[].name",value) //批量修改对象数组所有元素的name值等于value
             */

            var update = menuRoleUpdate.UpsetType == UpsetType.Add
                ? Builders<Menu>.Update.AddToSet(p => p.Meta.Roles,
                    menuRoleUpdate.RoleId)
                : Builders<Menu>.Update.Pull(p => p.Meta.Roles,
                    menuRoleUpdate.RoleId);
            return new UpdateOneModel<Menu>(Filter.Eq(x => x.Id, menuRoleUpdate.MenuId),
                update.CurrentDate(i => i.ModifiedOn)
            );
        }
    }
}