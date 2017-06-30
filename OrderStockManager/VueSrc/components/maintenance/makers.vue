<template lang="pug">
div
  el-table(:data="makers" height="480")
    el-table-column(prop="code" label="コード" sortable width="150")
    el-table-column(prop="name" label="名称")
    el-table-column(prop="enabled" label="使用許可" :filters="enabledFilters" :filter-method="filterEnabled" filter-placement="bottom-end" :filter-multiple="false" width="120")
      template(scope="scope")
        span(v-text="scope.row.enabled?'許可':'不許可'")
    el-table-column(label="機能" width="150")
      template(scope="scope")
        el-button(size="small" @click="changeMaker(scope.row)") {{scope.row.enabled ? '不許可':'許可'}}
    el-table-column(prop="deleted" label="削除" :filters="deletedFilters" :filter-method="filterDeleted" filter-placement="bottom-end" :filter-multiple="false" width="120")
      template(scope="scope")
        span {{scope.row.deleted | deletedMessage}}
</template>

<script>
import Enumerable from 'linq';
export default {
  metaInfo: function () {
    return {
      title: this.title
    }
  },
  data() {
    return {
      title: 'メーカー管理',
      enabledFilters: [{ text: '許可', value: 'true' }, { text: '不許可', value: 'false' }],
      deletedFilters: [{ text: '削除済み', value: 'true' }, { text: '未削除', value: 'false' }],
      makers: []
    }
  },
  methods: {
    filterEnabled(value, row) {
      return row.enabled.toString() === value
    },
    filterDeleted(value, row) {
      return row.deleted.toString() === value
    },
    changeMaker(maker) {
      this.$store.dispatch('nowLoadingMainte', 'メーカー情報更新中')
      maker.enabled = maker.enabled ? false : true
      this.$store.dispatch('maintenance/setMakers', maker).then((response) => {
        this.getMaker()
        this.$store.dispatch('endLoading')
      }).catch((error) => {
        this.$store.dispatch('endLoading')
        this.$notify.error({ title: 'Error', message: error.message })
      })
    },
    getMaker() {
      this.$store.dispatch('nowLoadingMainte', 'メーカー情報処理中')
      this.$store.dispatch('maintenance/getMakers').then((response) => {
        var items = this.minotaka.makeArray(response.data)
        this.makers = Enumerable.from(items).orderBy(x => x.code).toArray()
        this.$store.dispatch('endLoading')
      }).catch((error) => {
        this.$store.dispatch('endLoading')
        this.$notify.error({ title: 'Error', message: error.message })
      })
    }
  },
  created() {
    this.getMaker()
  },
  watch: {
    '$route': 'getMaker'
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
