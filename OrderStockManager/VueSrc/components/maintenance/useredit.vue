<template lang="pug">
div.basemargin
  el-form(:model="user" label-position="right" label-width="150px" :rules="checkRules" ref="userForm")
    el-form-item(label="サインインＩＤ" prop="userName")
      el-input(type="text" v-model="user.userName")
    el-form-item(label="ユーザー名" prop="name")
      el-input(type="text" v-model="user.name")
    el-form-item(label="メールアドレス" prop="email")
      el-input(type="email" v-model="user.email")
    el-form-item(label="メール認証")
      el-switch(v-model="user.emailConfirmed" on-text="済" off-text="未")
    el-form-item(label="有効期限")
      |{{user.expiration | converetDateFormat}}
      i.material-icons(style="font-size:13px; margin-left:8px; margin-right:8px;") &#xE5C8;
      el-date-picker(type="date" v-model="user.newExpiration")
    el-form-item(label="ロックアウト")
      el-date-picker(type="date" v-model="user.lockoutEndData")
    el-form-item(label="ロックアウト対象")
      el-switch(v-model="user.lockoutEnabled")
    el-form-item(label="連続入力ミス")
      el-input-number(v-model="user.accessFailedCount" :min=0 :max=10)
    el-form-item(label="パスワードスキップ")
      el-input-number(v-model="user.passwordSkipCnt" :min=0 :max=10)
    el-form-item(label="新パスワード" prop="newPassword")
      el-input(type="text" v-model="user.newPassword")
    el-form-item(label="使用許可")
      el-switch(v-model="user.enabled" on-text="許可" off-text="")
    el-form-item(label="削除")
      el-switch(v-model="user.deleted" on-text="削除" off-text="")
    el-form-item
      el-button(type="primary" @click="submitForm('userForm')") 変更
      el-button(@click="cancel") キャンセル
</template>

<script>
export default {
  props: ['id'],
  metaInfo: function () {
    return {
      title: this.title
    }
  },
  data() {
    return {
      title: 'ユーザー編集',
      user: {},
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
    getUser() {
      return this.$store.dispatch('maintenance/getUser', this.id).then((response) => {
        var item = response.data
        this.user = this.minotaka.makeSingleType(item, "object")
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
      })
    },
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.$store.dispatch('nowLoadingMainte', 'ユーザー情報更新中')
          this.$store.dispatch('maintenance/setUser', this.user).then((response) => {
            this.$notify({ title: '変更完了', message: 'ユーザー情報の更新を行いました' })
            this.$store.dispatch('endLoading')
            this.$router.go(-1)
          }).catch((error) => {
            this.$store.dispatch('endLoading')
            this.$notify.error({ title: 'Error', message: error.message })
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
      vm.$store.dispatch('nowLoadingMainte', 'ユーザー情報処理中')
      vm.$store.commit('changeBreadcrumb',
        { path: vm.$route.path, name: vm.title }
      )
      vm.getUser().then(() => {
        vm.$store.dispatch('endLoading')
      })
    })
  }
}
</script>

<style lang="scss" scoped>
.basemargin {
  margin: 8px
}
</style>
