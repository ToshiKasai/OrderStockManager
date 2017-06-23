<template lang="pug">
div
  div 管理機能へようこそ
  div ログイン者権限によりますが、左記メニューより機能を選択してください。
</template>

<script>
export default {
  metaInfo: {
    title: '管理機能',
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: '/mainte', name: '管理画面' }
      )
    })
  }
}
</script>

<style lang="scss" scoped>

</style>
