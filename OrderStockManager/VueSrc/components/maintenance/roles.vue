<template lang="pug">
div
  el-table(:data="tableData" v-loading="loading" element-loading-text="Loading...")
    el-table-column(prop="name" label="コード" sortable width="150")
    el-table-column(prop="displayName" label="説明")
    el-table-column(prop="deleted" label="削除" :filters="[{ text: '削除済み', value: true }, { text: '未削除', value: false }]" :filter-method="filterTag" filter-placement="bottom-end" width="120")
      template(scope="scope")
        span(v-text="scope.row.deleted?'削除済み':'－'")
</template>

<script>
export default {
  metaInfo: {
    title: 'ロール管理',
  },
  data() {
    return {
      loading: false
    }
  },
  computed: {
    tableData() {
      return this.$store.getters['maintenance/getRoleList']
    }
  },
  methods: {
    filterTag(value, row) {
      return row.deleted === value
    },
    getRoles(){
      this.loading = true
      this.$store.dispatch('maintenance/getRoles').then(() => { this.loading = false })
    }
  },
  created() {
    this.getRoles()
  },
  watch: {
    '$route': 'getRoles'
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('changeBreadcrumb',
        { path: '/roles', name: 'ロール' }
      )
    })
  },
  beforeRouteLeave(to, from, next) {
    this.$store.commit('maintenance/setRoles', [])
    next()
  }
}
</script>

<style lang="scss" scoped>
.fulltable {
  width: 100%;
}
</style>
