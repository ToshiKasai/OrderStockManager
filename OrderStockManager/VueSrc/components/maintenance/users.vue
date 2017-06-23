<template lang="pug">
div
  el-table(:data="tableData" v-loading="loading" element-loading-text="Loading...")
    el-table-column(prop="userName" label="コード" sortable width="150")
    el-table-column(prop="name" label="ユーザー名")
    el-table-column(prop="expiration" label="有効期限" sortable)
      template(scope="scope")
        span(v-text="converetDateFormat(scope.row.expiration)")
    el-table-column(prop="email" label="メールアドレス")
    el-table-column(prop="deleted" label="削除" :filters="[{ text: '削除済み', value: true }, { text: '未削除', value: false }]" :filter-method="filterTag" filter-placement="bottom-end" width="120")
      template(scope="scope")
        span(v-text="scope.row.deleted?'削除済み':'－'")
    el-table-column(label="機能" width="150")
      template(scope="scope")
        el-button(size="small" type="text") role
        el-button(size="small" type="text") maker
</template>

<script>
// import { dateToFormatString } from '../../libraries/dateToFormatString';
import moment from 'moment'
moment.locale('ja', {
  weekdays: ["日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"],
  weekdaysShort: ["日", "月", "火", "水", "木", "金", "土"]
})

export default {
  metaInfo: {
    title: 'ユーザー管理',
  },
  data() {
    return {
      loading: false
    }
  },
  computed: {
    tableData() {
      return this.$store.getters['maintenance/getUserList']
    }
  },
  methods: {
    filterTag(value, row) {
      return row.deleted === value
    },
    converetDateFormat(date) {
      // var tmp = new Date(date)
      // return dateToFormatString(tmp, '%YYYY%/%MM%/%DD%(%w%)')
      var tmp = moment(date)
      return tmp.format('YYYY/MM/DD(ddd)')
    },
    getUsers(){
      this.loading = true
      this.$store.dispatch('maintenance/getUsers').then(() => { this.loading = false })
    }
  },
  created() {
    this.getUsers()
  },
  watch: {
    '$route': 'getUsers'
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: '/users', name: 'ユーザー' }
      )
    })
  },
  beforeRouteLeave(to, from, next) {
    this.$store.commit('maintenance/setUsers', [])
    next()
  }
}
</script>

<style lang="scss" scoped>
.fulltable {
  width: 100%;
}
</style>
