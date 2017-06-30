<template lang="pug">
div
  el-table(:data="roles")
    el-table-column(prop="name" label="コード" sortable width="150")
    el-table-column(prop="displayName" label="説明")
    el-table-column(prop="deleted" label="削除" :filters="disabledFilters" :filter-method="filterDisabled" filter-placement="bottom-end" :filter-multiple="false" width="120")
      template(scope="scope")
        span(v-text="scope.row.deleted?'削除済み':'－'")
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
      title: 'ロール管理',
      roles: [],
      disabledFilters: [{ text: '削除', value: 'true' }, { text: '未削除', value: 'false' }]
    }
  },
  methods: {
    filterDisabled(value, row) {
      return row.deleted.toString() === value
    },
    getRoles() {
      this.$store.dispatch('nowLoadingMainte', 'ロール情報取得中')
      this.$store.dispatch('maintenance/getRoles').then((response) => {
        var items = response.data
        this.roles = this.minotaka.makeArray(items)
        this.$store.dispatch('endLoading')
      }).catch((error) => {
        this.$store.dispatch('endLoading')
        this.$notify.error({ title: 'Error', message: error.message })
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
        { path: vm.$route.path, name: vm.title }
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
