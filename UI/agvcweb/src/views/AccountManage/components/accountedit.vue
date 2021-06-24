<template>
    <div>
        <el-tabs v-model="activeName">
            <el-tab-pane label="基础信息" name="first">
                <div>
                    <el-form ref="sysuser" :model="sysuser" :rules="rules" label-width="80px" label-position="left">
                        <el-form-item label="用户性质" prop="forbiddenLogin" v-if="dialogType !== 'edit'">
                            <el-switch v-model="sysuser.forbiddenLogin" active-text="非登陆用户" />
                        </el-form-item>

                        <el-form-item v-if="!sysuser.forbiddenLogin" label="用户账号" prop="loginId">
                            <el-input v-model="sysuser.loginId" placeholder="请输入账号" />
                        </el-form-item>

                        <el-form-item v-if="dialogType !== 'edit' && !sysuser.forbiddenLogin" label="账户密码"
                            prop="loginPwd">
                            <PasswordStrengthMeter :pwd="sysuser.loginPwd" @changevlue="changevlue">
                            </PasswordStrengthMeter>
                        </el-form-item>

                        <el-form-item v-if="dialogType === 'edit' && !sysuser.forbiddenLogin" label="账户密码">
                            <PasswordStrengthMeter :pwd="sysuser.loginPwd" @changevlue="changevlue">
                            </PasswordStrengthMeter>
                        </el-form-item>
                        <el-form-item v-if="dialogType !== 'edit' && !sysuser.forbiddenLogin" label="首登"
                            prop="needChangePassword">
                            <el-switch v-model="sysuser.needChangePassword" active-text="首次登陆要求修改密码" />
                        </el-form-item>
                        <el-form-item label="用户性质" v-if="dialogType === 'edit' && sysuser.forbiddenLogin">
                            非登陆用户
                        </el-form-item>
                        <el-form-item label="姓名" prop="nick">
                            <el-input v-model="sysuser.nick" placeholder="" style="width:217px;" />
                        </el-form-item>
                        <el-form-item label="所属角色" prop="roleId" v-if="!sysuser.isshow">
                            <el-select v-model="sysuser.roleId" placeholder="请选择" value-key="id">
                                <el-option v-for="role in rolelist" :key="role.id" :value="role.id"
                                    :label="role.name" />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="">
                            <el-button icon="el-icon-check" type="primary" @click="confirmUser('sysuser')">
                                确认
                            </el-button>
                        </el-form-item>
                    </el-form>
                </div>
            </el-tab-pane>
            <el-tab-pane label="用户授权" name="second" v-if="!sysuser.isshow">
                <div v-if="sysuser.id">
                    <el-form label-position="left">
                        <el-form-item>
                            <RoleMenuTree v-model="sRoleMenus" :role-id="sysuser.roleId" :userId="sysuser.id"
                                :Mtype="false" />
                        </el-form-item>
                        <el-form-item label="">
                            <el-button icon="el-icon-check" type="primary" @click="handleuserAuthority">
                                确认
                            </el-button>
                        </el-form-item>
                    </el-form>
                </div>
                <div v-else>
                    需要先创建用户，才可以继续编辑用户权限。
                </div>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
    import { updateUserAuthority } from "@/api/role";
    import { mapAccountActions } from "@/store/namespaced/Account";
    import PasswordStrengthMeter from "@/components/PasswordStrengthMeter";
    import RoleMenuTree from "@/views/permission/components/RoleMenuTree";
    function initState() {
        return {
            id: "",
            loginId: "",
            loginPwd: "",
            roleId: "",
            roleName: "",
            nick: "",
            needChangePassword: true,
            forbiddenLogin: false, // 禁止登陆 /// 单纯创建用户，不设置用户名和密码
            branchCompany: "",
            dDepartment: "",
        };
    }
    export default {
        name: "accountedit",
        props: {
            ws: Object,
        },
        components: { PasswordStrengthMeter, RoleMenuTree },
        data() {
            return {
                activeName: "first",
                sysuser: initState(),
                rolelist: [],
                rules: {
                    loginId: [
                        // 表单手机号码格式验证
                        {
                            required: true,
                            message: "请输入账号！",
                            trigger: "blur",
                        },
                    ],
                    loginPwd: [
                        {
                            required: true,
                            message: "请输入密码！",
                            trigger: "blur",
                        },
                    ],
                    needChangePassword: [
                        {
                            required: true,
                            message: "请选择是否首次登陆强制修改密码",
                            trigger: "blur",
                        },
                    ],
                    forbiddenLogin: [
                        {
                            required: true,
                            message: "请选择用户性质",
                            trigger: "blur",
                        },
                    ],
                    nick: [
                        {
                            required: true,
                            message: "请输入姓名！",
                            trigger: "blur",
                        },
                    ],
                    roleId: [
                        {
                            required: true,
                            message: "请选择角色",
                            trigger: "change",
                        },
                    ],
                },
                sRoleMenus: [],
                userAuthority: {
                    authorizes: [],
                    roleId: '',
                    userId: ''
                },
                Isallow: false,
                dialogType: '',
  
            }
        },
        computed: {
      
        },
        async created() {
            this.sysuser = this.ws.modeldata;
            this.rolelist = this.ws.rolelist;
            this.dialogType = this.ws.dialogType;
        },
        methods: {
            updateUserAuthority,
            ...mapAccountActions([
                "QueryAccountUsersAsync",
                "CreateAccountUserAsync",
                "UpdateAccountUserAsync",
                "DeleteAccountUserAsync",
            ]),
            changevlue(arg) {
                this.sysuser.loginPwd = arg.pwd;
                this.Isallow = arg.Isallow;
            },
            //保存用户权限
            async handleuserAuthority() {
                this.userAuthority.authorizes = this.sRoleMenus;
                this.userAuthority.roleId = this.sysuser.roleId;
                this.userAuthority.userId = this.sysuser.id;
                var a = await this.updateUserAuthority(this.userAuthority);
                if (a) {
                    this.$message({
                        type: "success",
                        message: "操作成功!",
                    });
                    this.dialogVisible = false;
                }
            },
            // 新增或者编辑系统用户提交
            async confirmUser(formName) {
                this.$refs[formName].validate(async (valid) => {
                    if (valid) {
                        const isEdit = this.dialogType === "edit";
                        if (isEdit) {
                            // 修改系统用户 
                            if (this.sysuser.loginPwd) {
                                if (!this.Isallow) {
                                    this.$message({
                                        type: "error",
                                        message: "密码不合规则!",
                                    });
                                    return;
                                }
                            }
                            const user = await this.UpdateAccountUserAsync(this.sysuser);
                            if (user) {
                                this.$message({
                                    type: "success",
                                    message: "修改成功!",
                                });
                                this.ws.callback();
                                this.$close();
                            }
                        } else {
                            if (!this.Isallow) {
                                this.$message({
                                    type: "error",
                                    message: "密码不合规则!",
                                });
                                return;
                            }
                            const account = await this.CreateAccountUserAsync(this.sysuser);
                            if (account) {
                                this.$message({
                                    type: "success",
                                    message: "新增成功!",
                                });
                                this.ws.callback();
                                this.$close();
                            }
                        }
                    } else {
                        return false;
                    }
                });
            },


        }
    }

</script>