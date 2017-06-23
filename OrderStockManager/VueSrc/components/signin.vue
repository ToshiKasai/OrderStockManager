<template lang="pug">
div
  el-form(:model="signinForm" :rules="checkRules" ref="signinForm" v-loading.body="loading" element-loading-text="サインイン中...")
    el-form-item(label="サインインＩＤ" prop="inputId")
      el-input(type="text" v-model="signinForm.inputId")
    el-form-item(label="パスワード" prop="password")
      el-input(type="password" v-model="signinForm.password")
    el-form-item
      el-button(type="primary" @click="submitForm('signinForm')") サインイン
      el-button(@click="resetForm('signinForm')") リセット
  el-dialog(title="サインインに失敗しました" :visible.sync="dialogVisible" size="tiny")
    span サインインに失敗しました
    div(v-text="dialogMessage")
    span.dialog-footer(slot="footer")
      el-button(type="primary" @click="dialogVisible = false") OK
</template>

<script>
export default {
  metaInfo: {
    title: 'サインイン',
  },
  data() {
    var validateId = (rule, value, callback) => {
      if (!/^[a-zA-Z\d@_]{5,100}$/.test(value)) {
        callback(new Error('不正なサインインＩＤです'))
      }
      callback()
    };
    var validaePass = (rule, value, callback) => {
      // if (!/^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?\d)[a-zA-Z\d]{6,100}$/.test(value)) {
      if (!/^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?\d)[[!-~]{6,100}$/.test(value)) {
        callback(new Error('不正なパスワードです'))
      }
      callback()
    };
    return {
      signinForm: {
        inputId: '',
        password: ''
      },
      checkRules: {
        inputId: [
          { required: true, message: '入力は必須です', trigger: 'blur' },
          { validator: validateId, trigger: 'blur' }
        ],
        password: [
          { required: true, message: 'パスワードを入力してください', trigger: 'blur' },
          { validator: validaePass, trigger: 'blur' }
        ]
      },
      dialogVisible: false,
      dialogMessage: "",
      loading: false
    };
  },
  computed: {
  },
  methods: {
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.loading = true
          this.$store.dispatch('postSignin', this.signinForm).then((value) => {
            this.loading = false
            if (this.$store.state.nameid === 0) {
              this.dialogVisible = true
              this.dialogMessage = "入力内容を見直してください"
            } else {
              this.$router.push('/menu')
            }
          })
        } else {
          return false
        }
      });
    },
    resetForm(formName) {
      this.$refs[formName].resetFields()
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: '/signin', name: 'サインイン' }
      )
    })
  }
}
</script>

<style lang="scss" scoped>

</style>
