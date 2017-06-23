<template lang="pug">
div
  el-row(:gutter="20")
    el-col(:span="8")
      el-button(size="large" style="width:100%") 在庫・販売状況確認
    el-col(:span="8")
      el-button(size="large" style="width:100%") 予算データアップロード
    el-col(:span="8")
      el-button(size="large" style="width:100%") パスワード変更
  el-row(:gutter="20")
    el-col(:span="8")
      el-button(size="large" style="width:100%") メールアドレス変更
    el-col(:span="8")
      el-button(size="large" style="width:100%" @click="goMainte") 管理機能
    el-col(:span="8")
      el-button(size="large" style="width:100%") サインインログ表示
  el-row(:gutter="20")
    el-col(:span="8")
      el-button(size="large" style="width:100%") アクションログ表示
  hr
  el-table(:data="tableData" v-loading="loading" element-loading-text="Loading...")
    el-table-column(prop="startDateTime" label="掲載日" width="180")
      template(scope="scope")
        span(v-text="converetDateFormat(scope.row.startDateTime)")
    el-table-column(prop="message" label="メッセージ")
</template>

<script>
import moment from 'moment'
moment.locale('ja', {
  weekdays: ["日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"],
  weekdaysShort: ["日", "月", "火", "水", "木", "金", "土"]
})

export default {
  metaInfo: {
    title: 'メニュー',
  },
  data: function () {
    return {
      loading: false
    }
  },
  computed: {
    tableData() {
      return this.$store.state.dashboard
    }
  },
  methods: {
    goMainte() {
      this.$router.push('/mainte')
    },
    converetDateFormat(date) {
      var tmp = moment(date)
      return tmp.format('YYYY/MM/DD(ddd)')
    },
    getDashboard(){
      this.loading = true
      this.$store.dispatch('getDashboard').then(() => { this.loading = false })
    }
  },
  created() {
    this.$store.commit('setBreadcrumb',
      { path: '/menu', name: 'MENU' }
    )
    this.getDashboard()
  },
  watch: {
    '$route': 'getDashboard'
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: '/menu', name: 'MENU' }
      )
    })
  },
  beforeRouteLeave(to, from, next) {
    this.$store.commit('setDashboard', [])
    next()
  }
}
</script>

<style lang="scss" scoped>

</style>
