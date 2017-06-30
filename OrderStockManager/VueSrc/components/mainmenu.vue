<template lang="pug">
div
  el-row(:gutter="20")
    el-col(:span="8")
      el-button(size="large" @click="goSelect") 在庫・販売状況確認
    el-col(:span="8")
      el-button(size="large") 予算データアップロード
    el-col(:span="8")
      el-button(size="large") パスワード変更
  el-row(:gutter="20")
    el-col(:span="8")
      el-button(size="large") メールアドレス変更
    el-col(:span="8")
      el-button(size="large" @click="goMainte") 管理機能
    el-col(:span="8")
      el-button(size="large") サインインログ表示
  el-row(:gutter="20")
    el-col(:span="8")
      el-button(size="large") アクションログ表示
  hr
  el-table(:data="dashboards" v-loading="loading" element-loading-text="Loading...")
    el-table-column(prop="startDateTime" label="掲載日" width="180")
      template(scope="scope")
        span {{scope.row.startDateTime | converetDateFormat}}
    el-table-column(prop="message" label="メッセージ")
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
      title: 'メニュー',
      loading: false,
      dashboards: []
    }
  },
  methods: {
    goSelect() {
      this.$router.push('/makerselect')
    },
    goMainte() {
      this.$router.push('/mainte')
    },
    getDashboard() {
      this.loading = true
      this.$store.dispatch('getDashboard').then((response) => {
        var items = response.data
        this.dashboards = this.minotaka.makeArray(items)
        this.loading = false
      }).catch((error) => {
        this.$notify({ title: 'Warning', message: error.message, type: 'warning' })
        this.loading = false
      })
    }
  },
  created() {
    this.$store.commit('setBreadcrumb',
      { path: this.$route.path, name: this.title }
    )
    this.getDashboard()
  },
  watch: {
    '$route': 'getDashboard'
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
