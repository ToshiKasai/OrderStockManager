<template lang="pug">
div
  div 管理機能へようこそ
  div ログイン者権限によりますが、左記メニューより機能を選択してください。
</template>

<script>
export default {
  metaInfo: function () {
    return {
      title: this.title
    }
  },
  data: function () {
    return {
      title: '管理機能'
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
