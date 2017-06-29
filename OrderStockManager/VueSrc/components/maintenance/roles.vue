<template lang="pug">
div
  el-table(:data="roles" v-loading="loading" element-loading-text="Loading...")
    el-table-column(prop="name" label="コード" sortable width="150")
    el-table-column(prop="displayName" label="説明")
    el-table-column(prop="deleted" label="削除" :filters="disabledFilters" :filter-method="filterDisabled" filter-placement="bottom-end" :filter-multiple="false" width="120")
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
      loading: false,
      roles: [],
      disabledFilters: [{ text: '削除', value: 'true' }, { text: '未削除', value: 'false' }]
    }
  },
  computed: {
  },
  methods: {
    filterDisabled(value, row) {
      return row.deleted.toString() === value
    },
    getRoles() {
      this.loading = true
      this.$store.dispatch('maintenance/getRoles').then((response) => {
        var items = response.data
        this.roles = this.minotaka.makeArray(items)
        this.loading = false
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
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
        { path: '/mainte/roles', name: 'ロール' }
      )
    })
  }
}
</script>

<style lang="scss" scoped>
.fulltable {
  width: 100%;
}
</style>
