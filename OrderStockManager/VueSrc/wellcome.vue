<template lang="pug">
div
  h4 ようこそ
  div サインインを行い、システムの利用を開始してください。
  div アカウントがない場合は、ワークフローで業務アカウント申請を行ってください。
  hr
  div 開発環境はGoogle Chromeが中心となります。
  div IE11は対応は考慮しておりますが、全機能の動作は確認しておりません。
  div モバイル環境のレイアウトは考慮しておりません。
  el-table(:data="browsers")
    el-table-column(prop="title" label="項目")
    el-table-column(prop="value" label="データ")
</template>

<script>
import platform from 'platform'
export default {
  metaInfo: function () {
    return {
      title: this.title
    }
  },
  data() {
    function browserData() {
      return [
        { title: 'ブラウザ', value: platform.name },
        { title: 'バージョン', value: platform.version },
        { title: 'レイアウトエンジン', value: platform.layout },
        { title: 'ＯＳ', value: platform.os.family }
      ]
    }
    return {
      title: 'Welcome',
      browsers: browserData()
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
