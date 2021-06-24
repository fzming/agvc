import store from "@/store";
export const hasCodePermission = (value) => {
  const codes = store.getters && store.getters.role_codes;
  console.log("=======================", codes, value);
  if (value && value.indexOf("zl-") > -1) {
    const codepermisson = codes.find((p) => p.code === value);
    console.log("codepermisson", codepermisson);
    if (codepermisson && codepermisson.disabled) return true; //指令已废除
    return codepermisson && codepermisson.hasPermission;
  }
  return false;
};

export const zl_data = {
  zl_Specialcosts: "zl-73a2540448774d5f84f90b6acd74b461", // 是否显示专项费用
  zl_dumpSpecialcosts: "zl-38d85e34c88a4649a80a29c8ef4b9e19", //是否显示专项费用（堆场）
  zl_orderlockStatusclose: "zl-48ee9cac303044be8897fd6ba557996f", // 订单锁定
  zl_orderlockStatusopen: "zl-ae608a446591491ea71bf51aa39cf5a0", // 订单解锁
  zl_costslock: "zl-3e76f1574f9b4043a3ae3237d4be71da", // 费用锁定
  zl_costsUnlock: "zl-7136377c16df458bb0399d84fb9fee12", // 费用解锁
  zl_costsSplit: "zl-08462bdccbd744a5b006a82fe51400b4", //费用拆分
  zl_hasSpecialCosts: "zl-e1b0aef4083c4091a0956b6ac50fcbd1",
  zl_clientCreat: "zl-627b7c78ff534f9dafced202725c3096", //创建-保存往来单位权限
  zl_clientSave: "zl-0ca44e87a89448babc077cec47cd2325", //修改-保存往来单位权限
  zl_Viewnonself:"zl-389a4f4b648c4e34a01d0f86e7279c09",//允许查看非本人业务
};

// 角色等级集合
export const roleGrade = {
  sysrolw: [
    // { name: '等级100', value: 100 },
    {
      name: "等级99",
      value: 99,
    },
    {
      name: "等级98",
      value: 98,
    },
    {
      name: "等级97",
      value: 97,
    },
    {
      name: "等级96",
      value: 96,
    },
    {
      name: "等级95",
      value: 95,
    },
    {
      name: "等级94",
      value: 94,
    },
    {
      name: "等级93",
      value: 93,
    },
    {
      name: "等级92",
      value: 92,
    },
    {
      name: "等级91",
      value: 91,
    },
    {
      name: "等级90",
      value: 90,
    },
  ], // 系统角色等级
  orgrolw: [
    {
      name: "等级0",
      value: 0,
    },
    {
      name: "等级1",
      value: 1,
    },
    {
      name: "等级2",
      value: 2,
    },
    {
      name: "等级3",
      value: 3,
    },
    {
      name: "等级4",
      value: 4,
    },
    {
      name: "等级5",
      value: 5,
    },
    {
      name: "等级6",
      value: 6,
    },
    {
      name: "等级7",
      value: 7,
    },
    {
      name: "等级8",
      value: 8,
    },
    {
      name: "等级9",
      value: 9,
    },
    {
      name: "等级10",
      value: 10,
    },
  ],
};
