<template lang="pug">
div
  el-row
    el-col(:xs="8" :sm="6" :md="4" :lg="3")
      el-menu(mode="vertical" theme="dark" :router="true")
        el-menu-item(index="/mainte") 管理メニュー
        el-menu-item(index="/mainte/users") ユーザーメンテ
        el-menu-item(index="/mainte/roles") ロールメンテ
        el-menu-item(index="/mainte/makers") メーカーメンテ
        el-menu-item(index="/mainte/products") 商品メンテ
        el-menu-item(index="/mainte/groups") グループメンテ
        el-menu-item(index="/mainte/dashboards") ダッシュボードメンテ
    el-col(:xs="16" :sm="18" :md="20" :lg="21" v-loading="nowLoading" :element-loading-text="loadingMessage")
      router-view
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
      title: '管理機能'
    }
  },
  computed: {
    nowLoading: function () {
      return this.$store.getters.nowLoadingMainte
    },
    loadingMessage: function () {
      return this.$store.getters.loadingMessage
    }
  }
}
</script>

<style lang="scss" scoped>

</style>
