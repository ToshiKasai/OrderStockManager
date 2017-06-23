<template lang="pug">
div
  h4 ようこそ
  div サインインを行い、システムの利用を開始してください。
  div アカウントがない場合は、ワークフローで業務アカウント申請を行ってください。
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
