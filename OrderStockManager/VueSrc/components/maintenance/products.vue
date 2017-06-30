<template lang="pug">
div
  data-tables(:data="products" :border="false"
    :search-def="searchDefine"
    :pagination-def="pageDefine" :has-action-col='false'
    :checkbox-filter-def="filterDefine")
    el-table-column(prop="code" label="コード" sortable width="110")
    el-table-column(prop="name" label="名称")
    el-table-column(prop="makerCode" label="メーカー" sortable)
      template(scope="scope")
        span {{scope.row.makerCode}} - {{scope.row.makerName}}
    el-table-column(prop="quantity" label="入り数" width="90" align="right")
    el-table-column(prop="isSoldWeight" label="計量" width="90")
      template(scope="scope")
        span(v-text="scope.row.isSoldWeight?'計量':'－'")
    el-table-column(prop="enabled" label="使用" width="90")
      template(scope="scope")
        span(v-text="scope.row.enabled?'許可':'不許可'")
    el-table-column(prop="deleted" label="削除" width="90")
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
      title: '商品管理',
      searchDefine: { props: ['code', 'name'], placeholder: 'コードと名称で絞り込む' },
      pageDefine: { pageSize: 10, pageSizes: [10, 15, 30, 50] },
      filterDefine: {
        props: ['isSoldWeight', 'enabled', 'deleted'],
        def: [
          { name: '削除', code: 'del.true' }, { name: '未削除', code: 'del.false' },
          { name: '許可', code: 'enb.true' }, { name: '不許可', code: 'enb.false' },
          { name: '計量', code: 'sol.true' }, { name: 'ピース', code: 'sol.false' }],
        filterFunction: this.myFilters
      },
      products: []
    }
  },
  methods: {
    myFilters(row, props) {
      var list = this.minotaka.makeArray(props.val, "string")
      var filter = [null, null, null]
      var ret = [true, true, true]
      for (var i = 0; i < list.length; i++) {
        var fils = list[i].split('.')
        if (fils[0] === "del") { if (filter[0] === null) { filter[0] = fils[1] } else { filter[0] = null } }
        if (fils[0] === "enb") { if (filter[1] === null) { filter[1] = fils[1] } else { filter[1] = null } }
        if (fils[0] === "sol") { if (filter[2] === null) { filter[2] = fils[1] } else { filter[2] = null } }
      }
      if (filter[0] !== null && row.deleted.toString() !== filter[0]) { ret[0] = false }
      if (filter[1] !== null && row.enabled.toString() !== filter[1]) { ret[1] = false }
      if (filter[2] !== null && row.isSoldWeight.toString() !== filter[2]) { ret[2] = false }
      return ret[0] && ret[1] && ret[2]
    },
    getProducts() {
      this.$store.dispatch('nowLoadingMainte', '商品情報処理中')
      this.$store.dispatch('maintenance/getProducts').then((response) => {
        var items = this.minotaka.makeArray(response.data)
        this.products = Enumerable.from(items).orderBy(x => x.code).toArray()
        this.$store.dispatch('endLoading')
      }).catch((error) => {
        this.$store.dispatch('endLoading')
        this.$notify.error({ title: 'Error', message: error.message })
      })
    }
  },
  created() {
    this.getProducts()
  },
  watch: {
    '$route': 'getProducts'
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
