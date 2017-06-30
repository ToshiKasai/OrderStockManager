<template lang="pug">
div
  el-form(:model="signinForm" :rules="checkRules" ref="signinForm" onSubmit="return false;")
    el-form-item(label="サインインＩＤ" prop="inputId")
      el-input(type="text" v-model="signinForm.inputId")
    el-form-item(label="パスワード" prop="password")
      el-input(type="password" v-model="signinForm.password")
    el-form-item
      el-button(type="primary" @click="submitForm('signinForm')" native-type="submit") サインイン
      el-button(@click="resetForm('signinForm')") リセット
  el-dialog(title="サインインに失敗しました" :visible.sync="dialogVisible" size="tiny")
    span サインインに失敗しました
    div(v-text="dialogMessage")
    span.dialog-footer(slot="footer")
      el-button(type="primary" @click="dialogVisible = false") OK
</template>

<script>
export default {
  props: ['redirect'],
  metaInfo: function () {
    return {
      title: this.title
    }
  },
  data() {
    return {
      title: 'サインイン',
      signinForm: {
        inputId: '',
        password: ''
      },
      checkRules: {
        inputId: [
          { type: "string", required: true, message: '入力は必須です', trigger: 'blur' }
        ],
        password: [
          { type: "string", required: true, message: 'パスワードを入力してください', trigger: 'blur' }
        ]
      },
      dialogVisible: false,
      dialogMessage: ""
    }
  },
  methods: {
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.$store.dispatch('nowLoadingFull', 'サインイン中...')
          this.$store.dispatch('signin', this.signinForm).then(() => {
            this.$store.dispatch('endLoading')
            if (this.$store.getters.isAuthenticated) {
              if (this.redirect === undefined || this.redirect === "" || this.redirect === null) {
                this.$router.push('/menu')
              } else {
                this.$router.push(this.redirect)
              }
            } else {
              this.dialogVisible = true
              this.dialogMessage = "入力内容に誤りがあります。入力内容を見直してください"
            }
          })
          return true
        } else {
          return false
        }
      })
    },
    resetForm(formName) {
      this.$refs[formName].resetFields()
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: vm.$route.path, name: vm.title }
      )
    })
  }
}
</script>

<style lang="scss" scoped>

</style>
