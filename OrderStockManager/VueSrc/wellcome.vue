<template lang="pug">
div
  h4 ようこそ
  div サインインを行い、システムの利用を開始してください。
  div アカウントがない場合は、ワークフローで業務アカウント申請を行ってください。
  hr
  div IE11は対応は考慮しておりますが、全画面での動作は保証できません。
  div また、IE9以降であれば動作はするとは思われますが、IE11以外は動作保証外となります。
</template>

<script>
export default {
  metaInfo: {
    title: 'Welcome'
  },
  created() {
    this.$store.commit('setBreadcrumb',
      { path: '/', name: 'WELCOME' }
    )
    if (this.$store.getters.isAuthenticated) {
      this.$router.replace('/menu')
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: '/', name: 'WELCOME' }
      )
    })
  }
}
</script>

<style lang="scss" scoped>

</style>
