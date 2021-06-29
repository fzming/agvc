// webapi
export const SIG_EVENT = "$_SIGNALR_$";
export const NODE_ENV = process.env.NODE_ENV;
export const HASDELETEBTNS = process.env.NODE_ENV === "development";
export const NET_WORK_ERROR_URL = ["/syslogin", "/login","/m/login",];

//本地调试API地址
export const WEBAPI_BASE_URL = process.env.NODE_ENV === "development"
? 'http://localhost:5000' : 'http://gps.e6erp.com:8080'
//局域网主机调试
//WEBAPI_BASE_URL = 'http://192.168.10.72:8096'
export const WEBAPI_REQUEST_TIMEOUT = 1360000
export const ACCOUNT_TYPES = [
  {
    id: 0,
    name: "正常",
    financeType: 0,
  },
  {
    id: 2,
    name: "代收",
    financeType: 10,
  },
  {
    id: 3,
    name: "代付",
    financeType: 20,
  },
];
export const USER_TYPE = {
  Account: "Account",
  Admin: "Admin",
};
export const templateUnitTypes = {
  "-1": "订单箱型",
  10: "立方米",
  20: "吨",
  0: "其他",
};
export const unitTypesdata = {
  0: "其他",
  // 显示单位
  10: "立方米",
  20: "吨",
  30: "20GP",
  40: "40GP",
  50: "40HQ",
  51: "20HQ",
  52: "20TK",
  53: "40TK",
  54: "20RF",
  55: "20RH",
  56: "40RF",
  57: "40RH",
  58: "20OT",
  59: "40OT",
  60: "20FR",
  61: "40FR",
  62: "20HT",
  63: "40HT",
  64: "45GP",
  65: "45HQ",
  66: "45TK",
  67: "45RF",
  68: "45RH",
  69: "45OT",
  70: "45FR",
  71: "45HT",
};
export const FINANCE_TYPES = {
  0: "待对账",
  10: "对账待审核",
  20: "确认待开票",
  30: "开票待核销",
  35: "已收款待确认",
  40: "请款待审核",
  50: "请款已审核",
  55: "已付款待核销",
  60: "已核销",
};
export const FINANCE_NO_TYPES = {
  0: "默认",
  10: "对账编号",
  20: "账单确认编号",
  30: "发票号",
  40: "请款编号",
  50: "请款审核编号",
  60: "凭证号",
};
export const BOX_TYPES = [
  {
    label: "20GP",
    id: 30,
    check: true,
  },
  {
    label: "40GP",
    id: 40,
    check: false,
  },
  {
    label: "40HQ",
    id: 50,
    check: false,
  },
  {
    label: "20HQ",
    id: 51,
    check: false,
  },
  {
    label: "20TK",
    id: 52,
    check: false,
  },
  {
    label: "40TK",
    id: 53,
    check: false,
  },
  {
    label: "20RF",
    id: 54,
    check: false,
  },
  {
    label: "20RH",
    id: 55,
    check: false,
  },
  {
    label: "40RF",
    id: 56,
    check: false,
  },
  {
    label: "40RH",
    id: 57,
    check: false,
  },
  {
    label: "20OT",
    id: 58,
    check: false,
  },
  {
    label: "40OT",
    id: 59,
    check: false,
  },
  {
    label: "20FR",
    id: 60,
    check: false,
  },
  {
    label: "40FR",
    id: 61,
    check: false,
  },
  {
    label: "20HT",
    id: 62,
    check: false,
  },
  {
    label: "40HT",
    id: 63,
    check: false,
  },
  {
    label: "45GP",
    id: 64,
    check: false,
  },
  {
    label: "45HQ",
    id: 65,
    check: false,
  },
  {
    label: "45TK",
    id: 66,
    check: false,
  },
  {
    label: "45RF",
    id: 67,
    check: false,
  },
  {
    label: "45RH",
    id: 68,
    check: false,
  },
  {
    label: "45OT",
    id: 69,
    check: false,
  },
  {
    label: "45FR",
    id: 70,
    check: false,
  },
  {
    label: "45HT",
    id: 71,
    check: false,
  },
];

export const SOURCE_TYPES = [
  {
    id: 0,
    value: "无",
  },
  {
    id: 1,
    value: "往来单位",
  },
  {
    id: 2,
    value: "费用名称",
  },
  {
    id: 3,
    value: "税率",
  },
  {
    id: 4,
    value: "费用所属业务段",
  },
  {
    id: 5,
    value: "账龄起始日类型",
  },
  {
    id: 6,
    value: "费用状态",
  },
  {
    id: 7,
    value: "业务人员",
  },
  {
    id: 8,
    value: "港口",
  },
  {
    id: 9,
    value: "箱型",
  },
  {
    id: 10,
    value: "货物名称",
  },
  {
    id: 11,
    value: "订单业务类型",
  },
];

//COST_NAME_DEFINES
//堆存费用
export const DUMPS_COST_NAME_DEFINES = [
  {
    value: "A箱费",
    label: "A箱费",
  },
  {
    value: "COD转港费",
    label: "COD转港费",
  },
  {
    value: "EIR打单费",
    label: "EIR打单费",
  },
  {
    value: "ERA空箱调运费",
    label: "ERA空箱调运费",
  },
  {
    value: "IPF集装箱检查费",
    label: "IPF集装箱检查费",
  },
  {
    value: "OWS超重附加费",
    label: "OWS超重附加费",
  },
  {
    value: "PSC港口拥挤附加费",
    label: "PSC港口拥挤附加费",
  },
  {
    value: "SCC安全附加费",
    label: "SCC安全附加费",
  },
  {
    value: "T.R.C码头收柜费",
    label: "T.R.C码头收柜费",
  },
  {
    value: "搬移费",
    label: "搬移费",
  },
  {
    value: "办单费",
    label: "办单费",
  },
  {
    value: "包干费",
    label: "包干费",
  },
  {
    value: "包装费",
    label: "包装费",
  },
  {
    value: "操作费",
    label: "操作费",
  },
  {
    value: "查验费",
    label: "查验费",
  },
  {
    value: "查验服务费",
    label: "查验服务费",
  },
  {
    value: "拆箱费",
    label: "拆箱费",
  },
  {
    value: "场站费",
    label: "场站费",
  },
  {
    value: "场站使用费",
    label: "场站使用费",
  },
  {
    value: "场站制单费",
    label: "场站制单费",
  },
  {
    value: "超期堆存费",
    label: "超期堆存费",
  },
  {
    value: "超期用箱费",
    label: "超期用箱费",
  },
  {
    value: "超重费",
    label: "超重费",
  },
  {
    value: "待时费",
    label: "待时费",
  },
  {
    value: "单证费",
    label: "单证费",
  },
  {
    value: "捣箱费",
    label: "捣箱费",
  },
  {
    value: "封志费",
    label: "封志费",
  },
  {
    value: "改单费",
    label: "改单费",
  },
  {
    value: "港建费",
    label: "港建费",
  },
  {
    value: "港务费",
    label: "港务费",
  },
  {
    value: "港杂费",
    label: "港杂费",
  },
  {
    value: "管理费",
    label: "管理费",
  },
  {
    value: "过磅费",
    label: "过磅费",
  },
  {
    value: "过夜费",
    label: "过夜费",
  },
  {
    value: "还箱费",
    label: "还箱费",
  },
  {
    value: "换箱费",
    label: "换箱费",
  },
  {
    value: "回空费",
    label: "回空费",
  },
  {
    value: "加班费",
    label: "加班费",
  },
  {
    value: "进仓费",
    label: "进仓费",
  },
  {
    value: "进出库费",
    label: "进出库费",
  },
  {
    value: "开门费",
    label: "开门费",
  },
  {
    value: "内装箱费",
    label: "内装箱费",
  },
  {
    value: "喷淋费",
    label: "喷淋费",
  },
  {
    value: "铺装费",
    label: "铺装费",
  },
  {
    value: "人工费",
    label: "人工费",
  },
  {
    value: "上下车费",
    label: "上下车费",
  },
  {
    value: "掏箱费",
    label: "掏箱费",
  },
  {
    value: "提箱费",
    label: "提箱费",
  },
  {
    value: "拖车费",
    label: "拖车费",
  },
  {
    value: "污箱费",
    label: "污箱费",
  },
  {
    value: "小费",
    label: "小费",
  },
  {
    value: "修箱费",
    label: "修箱费",
  },
  {
    value: "熏蒸费",
    label: "熏蒸费",
  },
  {
    value: "押金",
    label: "押金",
  },
  {
    value: "佣金",
    label: "佣金",
  },
  {
    value: "增值税",
    label: "增值税",
  },
  {
    value: "滞纳金",
    label: "滞纳金",
  },
  {
    value: "装箱费",
    label: "装箱费",
  },
];

// ORDER_DATE_TYPES
// 订单的时间类型
export const ORDER_DATE_TYPES = [
  {
    name: "离港日期",
    value: "2",
  },
  {
    name: "装货日期",
    value: "1",
  },
  {
    name: "下单日期",
    value: "0",
  },
  {
    name: "到港日期",
    value: "3",
  },
  {
    name: "送货日期",
    value: "4",
  },
];

export const MODULE_NAME_TYPE = [
  {
    name: "货代模块",
    value: 1,
    icon: "el-icon-school",
  },
  {
    name: "堆场模块",
    value: 2,
    icon: "el-icon-box",
  },
  {
    name: "车队模块",
    value: 3,
    icon: "el-icon-truck",
  },
];

export const BOX_STATUS = [
  {
    title: "不限箱型",
    value: 0,
  },
  {
    title: "A箱",
    value: 1,
  },
  {
    title: "好箱",
    value: 2,
  },
  {
    title: "坏箱",
    value: 3,
  },
  {
    title: "污箱",
    value: 4,
  },
];

export const BUSSNESS_TYPES = [
  {
    title: "内贸",
    value: 1,
  },
  {
    title: "外贸",
    value: 2,
  },
];

export const EMPTY_TYPE = [
  {
    title: "空箱",
    value: 1,
  },
  {
    title: "重箱",
    value: 2,
  },
];

export const SIDETYPE_TYPE = [
  {
    title: "默认",
    value: 0,
  },
  {
    title: "门端",
    value: 1,
  },
  {
    title: "左侧",
    value: 2,
  },
  {
    title: "右侧",
    value: 3,
  },
  {
    title: "前端",
    value: 4,
  },
  {
    title: "底面",
    value: 5,
  },
  {
    title: "顶面",
    value: 6,
  },
];
export const DANGER_TYPE = [
  {
    title: "0,非危险品",
    value: "0",
  },
  {
    title: "1.1,具有同时爆炸危险物质和物品 ",
    value: "11",
  },
  {
    title: "1.2,具有喷射危险但无重大爆炸危险的",
    value: "12",
  },
  {
    title: "1.3,具有燃烧危险或者较小爆炸或者喷",
    value: "13",
  },
  {
    title: "1.4,无重大危险的物质和物品 ",
    value: "14",
  },
  {
    title: "1.5,具有同时爆炸危险但很不敏感货物 ",
    value: "15",
  },
  {
    title: "1.6,没有整体爆炸危险的极不敏感物品 ",
    value: "16",
  },
  {
    title: "2.1,易燃气体",
    value: "21",
  },
  {
    title: "2.2,非易燃气体",
    value: "22",
  },
  {
    title: "2.3,有毒气体",
    value: "23",
  },
  {
    title: "3,易燃液体",
    value: "30",
  },
  {
    title: "4.1,易燃固体",
    value: "41",
  },
  {
    title: "4.2,易自燃物质 ",
    value: "42",
  },
  {
    title: "4.3,遇水放出易燃气体的物质",
    value: "43",
  },
  {
    title: "5.1,氧化物质",
    value: "51",
  },
  {
    title: "5.2,有机过氧化物",
    value: "52",
  },
  {
    title: "6.1,有毒物质",
    value: "61",
  },
  {
    title: "6.2,感染性物质",
    value: "62",
  },
  {
    title: "7,放射性物质",
    value: "70",
  },
  {
    title: "8,腐蚀品",
    value: "80",
  },
  {
    title: "9,杂类海洋污染物",
    value: "90",
  },
];
export const DUMP_BOX_TYPE = [
  "FR",
  "GH",
  "GP",
  "HC",
  "HQ",
  "HT",
  "OT",
  "RF",
  "RH",
  "TK",
];
export const CHARGEMODE_TYPE = [
  {
    title: "请选择",
    value: null,
  },
  {
    title: "包干计费",
    value: 0,
  },
  {
    title: "按固定天数计费",
    value: 1,
  },
  {
    title: "按阶梯天数计费",
    value: 2,
  },
];
export const CONTAINER_BOXTYPE = [
  "FR",
  "GH",
  "GP",
  "HC",
  "HQ",
  "HT",
  "OT",
  "RF",
  "RH",
  "TK",
];
export const CONTAINER_BOXREPAIRCODE = [
  {
    title: "喷砂并油漆",
    value: 0,
  },
  {
    title: "空气吹清",
    value: 1,
  },
  {
    title: "调整",
    value: 2,
  },
  {
    title: "取消",
    value: 3,
  },
  {
    title: "化洗",
    value: 4,
  },
  {
    title: "除臭",
    value: 5,
  },
  {
    title: "润滑",
    value: 6,
  },
  {
    title: "重装",
    value: 7,
  },
  {
    title: "整形",
    value: 8,
  },
  {
    title: "去除胶带/粘纸",
    value: 9,
  },
  {
    title: "整形并补焊",
    value: 10,
  },
  {
    title: "Handling",
    value: 11,
  },
  {
    title: "安装",
    value: 12,
  },
  {
    title: "检查并报告",
    value: 13,
  },
  {
    title: "嵌补",
    value: 14,
  },
  {
    title: "加润滑油/脂",
    value: 15,
  },
  {
    title: "改装",
    value: 16,
  },
  {
    title: "重装标记",
    value: 17,
  },
  {
    title: "Retnove markings",
    value: 18,
  },
  {
    title: "Overlapping partial section",
    value: 19,
  },
  {
    title: "贴敷",
    value: 20,
  },
  {
    title: "补漆",
    value: 22,
  },
  {
    title: "部分翻新",
    value: 23,
  },
  {
    title: "Surface preparation and paint",
    value: 24,
  },
  {
    title: "贴补",
    value: 25,
  },
  {
    title: "重新排列",
    value: 26,
  },
  {
    title: "翻新",
    value: 27,
  },
  {
    title: "拆除并废弃",
    value: 28,
  },
  {
    title: "重做",
    value: 29,
  },
  {
    title: "紧固",
    value: 30,
  },
  {
    title: "翻新前修理",
    value: 31,
  },
  {
    title: "更换",
    value: 32,
  },
  {
    title: "拆装",
    value: 33,
  },
  {
    title: "Re-rate",
    value: 34,
  },
  {
    title: "蒸洗",
    value: 35,
  },
  {
    title: "地板磨砂",
    value: 36,
  },
  {
    title: "封胶/重新封胶",
    value: 37,
  },
  {
    title: "驳料加固",
    value: 38,
  },
  {
    title: "段补",
    value: 39,
  },
  {
    title: "临时修理",
    value: 40,
  },
  {
    title: "预防性保养",
    value: 41,
  },
  {
    title: "补焊",
    value: 42,
  },
  {
    title: "扫箱",
    value: 43,
  },
  {
    title: "水洗",
    value: 44,
  },
  {
    title: "打磨并补焊",
    value: 45,
  },
];
export const CURRENCYTYPE = [
  {
    title: "￥",
    value: 0,
  },
  {
    title: "$",
    value: 1,
  },
];
export const CONTAINERUNIT_TYPE = [
  {
    title: "个",
  },
  {
    title: "件",
  },
  {
    title: "套",
  },
  {
    title: "次",
  },
  {
    title: "M³",
  },
  {
    title: "CM³",
  },
];
export const FIX_MIAN = [
  {
    title: "箱门",
    url: "",
    value: 0,
  },
  {
    title: "箱门左侧",
    url: "",
    value: 1,
  },
  {
    title: "箱门右侧",
    url: "",
    value: 2,
  },
  {
    title: "箱尾",
    url: "",
    value: 3,
  },
  {
    title: "底面",
    url: "",
    value: 4,
  },
  {
    title: "顶面",
    url: "",
    value: 5,
  },
];

export const STATUETYPE_DATA = [
  {
    name: "待确认舱位",
    value: 0,
  },
  {
    name: "舱位已确认",
    value: 10,
  },
  {
    name: "装货已安排", //装货段车辆位置
    value: 15,
  },
  {
    name: "已装货待配船",
    value: 20,
  },
  {
    name: "已离港", //一程船位置
    value: 30,
  },
  {
    name: "已到港待送货",
    value: 40,
  },
  {
    name: "即将超期",
    value: 42,
  },
  {
    name: "已超期",
    value: 43,
  },
  {
    name: "送货已安排", //装货段车辆位置
    value: 45,
  },
  {
    name: "已送货",
    value: 50,
  },
];

export const GOODS_SOURCE = [
  {
    name: "集装箱",
    value: 1,
  },
  {
    name: "散货车",
    value: 3,
  },
];
export const GOODS_SOURCEII = [
  {
    name: "集装箱",
    value: 6,
  },
  {
    name: "散货车",
    value: 7,
  },
];

export const BOXES_SOURCE_EMPTY = [
  {
    name: "进闸空箱",
    value: 0,
  },
  {
    name: "在场空箱",
    value: 1,
  },
];

export const BOXES_SOURCE = [
  {
    name: "进闸重箱",
    value: 1,
  },
  {
    name: "在场重箱",
    value: 0,
  },
];

//  None=0,
//  集装箱改散货=1,
//  散货改集装箱 = 2,
//  箱对箱更换 = 3,
//  拆箱入库 = 4,
//  散货入库 = 5,
//  装箱出库 = 6,
//  散货出库 = 7,
export const OLTYPE = [
  {
    name: "请选择业务类型",
    value: 0,
  },
  {
    name: "集装箱改散货",
    value: 1,
  },
  {
    name: "散货改集装箱",
    value: 2,
  },
  {
    name: "箱对箱更换",
    value: 3,
  },
  {
    name: "拆箱入库",
    value: 4,
  },
  {
    name: "散货入库",
    value: 5,
  },
  {
    name: "装箱出库",
    value: 6,
  },
  {
    name: "散货出库",
    value: 7,
  },
];

export const PAY_UNITS = [
  { value: "pcs", title: "件" },
  { value: "gw", title: "吨" },
  { value: "cube", title: "m³" },
];

export const GLOBAL_ORDER_MODEL = function () {
  return {
    no: "", // 订单编号
    followNo: "", // 流水编号
    contractNo: "", // 合同编号
    Transit: "", // 运输条款
    CompanyId: "", // 往来单位ID(委托方)
    CompanyName: "", // 委托方名称
    Company: {
      // 委托方联系人信息
      id: "", // Id
      User: "", // 业务联系人
      Dep: "", // 部门
      Duty: "", // 职务
      Mobile: "", // 手机
      Phone: "", // 座机
      Fax: "", // 传真
      Mail: "", // 邮箱
      Addr: "", // 公司地址
      Remark: "", // 备注信息
    },
    loadAddress: {
      company: "",
      name: "",
      address: "",
      user: "",
      mobile: "",
      remark: "",
      addressType: 0,
    },
    deliveryAddress: {
      company: "",
      name: "",
      address: "",
      user: "",
      mobile: "",
      remark: "",
      addressType: 0,
    },
    loadCarrier: {
      // 装货段承运人
      name: "",
      id: "",
    },
    deliveryCarrier: {
      // 送货段承运人
      name: "",
      id: "",
    },

    Consigner: {
      // 发货人联系方式 派单时一起发送
      id: "", // Id
      User: "", // 业务联系人
      Dep: "", // 部门
      Duty: "", // 职务
      Mobile: "", // 手机
      Phone: "", // 座机
      Fax: "", // 传真
      Mail: "", // 邮箱
      Addr: "", // 公司地址
      Remark: "", // 备注信息
    },
    ConsigneeCompany: "", // 收货单位名称
    Consignee: {
      id: "", // Id
      User: "", // 业务联系人
      Dep: "", // 部门
      Duty: "", // 职务
      Mobile: "", // 手机
      Phone: "", // 座机
      Fax: "", // 传真
      Mail: "", // 邮箱
      Addr: "", // 公司地址
      Remark: "", // 备注信息
    },
    Cargo: {
      // 委托运输的货物(总PCS)
      Name: "", // 品名
      GW: "", // 毛重(吨)
      PCS: "", // 件数
      Cube: "", // 立方
      DangerType: "", // 危险类别
      UNNO: "", // 危险品编号（4位数字）
    },
    containerUnits: [
      {
        containerType: 30,
        count: 0,
      },
    ],
    CargoNote: "", // 货物备注
    Insurance: {
      // 保险信息
      CompanyId: "", // 保险公司ID
      CompanyName: "", // 保险公司名称
      Value: "", // 整票货值
      Rate: "", // 利率
      Total: "", // 应付保费 (默认：货值*利率)
    },
    CostTotal: "", // 应收总额
    PayableTotal: "", // 应付总额
    ProfitTotal: "", // 利润总额
    Shipping: {
      // 海运信息
      WaybillNumber: "", // 运单号
      Company: "", // 船公司名称
      CompanyId: "", // 公司往来单位ID
      Eta: "", // 到港日期(手工填写)
      Etd: "", // 离港日期(手工填写)
      LPortId: "", // 装货港
      LPortName: "", // 装货港名称
      DPortId: "", // 卸货港
      DPortName: "", // 卸货港名称
      Ships: [
        {
          MMSI: "",
          Name: "",
          Voyage: "",
        },
      ],
    },
    orgUserId: "", // 业务员Id
    orgUserName: "", // 业务员名称
    OperaUser: {
      // 制单人
      name: "",
      Id: "",
      Time: "", // 操作日期
      Extra: "", // 操作备注
    },
    LockStatus: {
      // 订单锁定状态 详细锁定解锁记录请参考订单日志表
      Locked: "", // 否已锁
      User: {
        Name: "",
        Id: "",
        Time: "", // 操作日期
        Extra: "", // 操作备注
      },
      loadTime: "", // 装货时间
      deliveryTime: "", // 送货时间
      Ymt: "", // 年月份标记，自动生成
    },
  };
};

export const COST_TYPES = [
  {
    title: "正常费用",
    value: "0",
  },
  {
    title: "内部费用",
    value: "1",
  },
  {
    title: "专项费用",
    value: "2",
  },
];

export const WXAPPID = "wxe2428ed94de8efd3"; //微信

export const COSTSEALTYPE = [
  { name: "包含", value: 0 },
  { name: "排除", value: 1 },
  { name: "仅显示", value: 2 },
];
