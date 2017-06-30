<template lang="pug">
div
  h4 ようこそ
  div サインインを行い、システムの利用を開始してください。
  div アカウントがない場合は、ワークフローで業務アカウント申請を行ってください。
  hr
  div 開発環境はGoogle Chromeが中心となります。
  div IE11は対応は考慮しておりますが、全機能の動作は確認しておりません。
</template>

<script>
export default {
  metaInfo: function () {
    return {
      title: this.title
    }
  },
  data() {
    return {
      title: 'Welcome'
    }
  },
  created() {
    this.$store.commit('setBreadcrumb',
      { path: this.$route.path, name: this.title }
    )
    if (this.$store.getters.isAuthenticated) {
      this.$router.replace('/menu')
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
