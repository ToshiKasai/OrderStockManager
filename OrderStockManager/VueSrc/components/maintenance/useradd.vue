<template lang="pug">
div.basemargin
  el-form(:model="user" label-position="right" label-width="150px" :rules="checkRules" ref="userForm")
    el-form-item(label="サインインＩＤ" prop="userName")
      el-input(type="text" v-model="user.userName")
    el-form-item(label="ユーザー名" prop="name")
      el-input(type="text" v-model="user.name")
    el-form-item(label="パスワード" prop="newPassword")
      el-input(type="text" v-model="user.newPassword")
    el-form-item(label="メールアドレス" prop="email")
      el-input(type="email" v-model="user.email")
    el-form-item
      el-button(type="primary" @click="submitForm('userForm')") 登録
      el-button(@click="cancel") キャンセル
</template>

<script>
export default {
  metaInfo: {
    title: 'ユーザー登録',
  },
  data() {
    return {
      user: {
        id: 0,
        userName: null,
        name: null,
        expiration: null,
        passwordSkipCnt: 0,
        email: null,
        emailConfirmed: false,
        lockoutEndData: null,
        lockoutEnabled: true,
        accessFailedCount: 0,
        enabled: true,
        deleted: false,
        newExpiration: null,
        newPassword: null
      },
      checkRules: {
        userName: [
          { type: "string", required: true, message: '入力は必須です', trigger: 'blur' },
          { type: "string", min: 5, max: 256, message: '桁数が正しくありません', trigger: 'blur' },
          { type: "string", pattern: /^[a-zA-Z\d@_]{5,256}$/, message: '不正な書式です', trigger: 'blur' }
        ],
        name: [
          { max: 256, message: '桁数が正しくありません', trigger: 'blur' }
        ],
        newPassword: [
          { type: "string", pattern: /^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?\d)[[!-~]{6,128}$/, message: '不正な書式です', trigger: 'blur' }
        ],
        email: [
          { type: "email", required: true, message: 'メールアドレスを入力してください', trigger: 'blur' },
          { max: 128, message: '桁数が正しくありません', trigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.$store.dispatch('nowLoadingMainte', 'ユーザー情報登録中')
          this.$store.dispatch('maintenance/addUser', this.user).then((response) => {
            this.$notify({ title: '登録完了', message: 'ユーザーの登録を行いました' })
            this.$store.dispatch('endLoading')
            this.$router.go(-1)
          }).catch((error) => {
            this.$notify.error({ title: 'Error', message: error.message })
            this.$store.dispatch('endLoading')
          })
        } else {
          return false
        }
      })
    },
    cancel() {
      this.$router.go(-1)
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: '/mainte/users/add', name: 'ユーザー登録' }
      )
    })
  }
}
</script>

<style lang="scss" scoped>
.basemargin {
  margin: 8px
}
</style>
