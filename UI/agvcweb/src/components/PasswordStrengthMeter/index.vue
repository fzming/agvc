<template>
    <div>
        <el-input v-model="lpwd" placeholder="请输入密码" :type="'password'" @change="pwdinput"></el-input>
        <div class="lnu_container">
            <p :class="{ length_valid: password_length }">密码长度：8~16</p>
            <p :class="{ lovercase_valid: contains_lovercase }">包含字母</p>
            <p :class="{ number_valid: contains_number }">包含数字</p>
        </div>
    </div>
</template>
<script>
    export default {
        name: 'PasswordStrengthMeter',
        props: {
            pwd: String,
        },
        data() {
            return {
                lpwd: '',
                password_length: false,
                contains_lovercase: false,
                contains_number: false,
                contains_uppercase: false,
                Isallow: false,
            }
        },
        watch: {
            lpwd() {
                this.p_len()
            }
        },
        created() {
            this.lpwd = this.pwd
        },
        methods: {
            pwdinput() {
                if (this.password_length === true && this.contains_lovercase === true && this.contains_number === true) {
                    this.Isallow = true
                }
                else { this.Isallow = false }
                this.$emit("changevlue", {
                    pwd: this.lpwd, Isallow: this.Isallow
                });
            },
            p_len() {
                if (this.lpwd) {
                    if (this.lpwd.length >= 8 && this.lpwd.length <= 16) {
                        this.password_length = true;
                    } else {
                        this.password_length = false;
                    }
                } else { this.password_length = false; }
                this.contains_lovercase = /[a-z]/.test(this.lpwd);
                this.contains_number = /\d/.test(this.lpwd);
                this.contains_uppercase = /[A-Z]/.test(this.lpwd);
                if (this.contains_uppercase) {
                    this.contains_lovercase = true
                }
            }
        }
    }
</script>

<style lang="scss" scoped>
    .lnu_container {
        display: block;
        width: 220px;
        height: auto;
        display: flex;
        justify-content: space-between;
    }

    .lnu_container p {
        width: auto;
        height: auto;
        padding: 5px;
        font-size: 10px;
        line-height: 1.2;
        text-align: center;
        border-radius: 2px;
        color: rgba(71, 87, 98, .8);
        background: linear-gradient(to right, #00AD7C 50%, #eee 50%);
        background-size: 201% 100%;
        background-position: right;
        -webkit-transition: background .3s;
        transition: background .3s;
    }

    .length_valid,
    .lovercase_valid,
    .number_valid,
    .uppercase_valid {
        background-position: left !important;
        color: rgba(255, 255, 255, .9) !important;
    }
</style>