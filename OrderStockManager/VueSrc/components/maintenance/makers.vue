<template lang="pug">
div
  el-table(:data="tableData" v-loading="loading" element-loading-text="Loading..." :default-sort="{prop:'code',order:'ascending'}")
    el-table-column(prop="code" label="コード" sortable width="150")
    el-table-column(prop="name" label="名称")
    el-table-column(prop="enabled" label="使用許可" :filters="[{ text: '許可', value: true }, { text: '不許可', value: false }]" :filter-method="filterEnabled" filter-placement="bottom-end" width="120")
      template(scope="scope")
        span(v-text="scope.row.enabled?'許可':'不許可'")
    el-table-column(label="機能" width="150")
      template(scope="scope")
        el-button(size="small" v-show="!scope.row.enabled" @click="changeMaker(scope.row)") 許可
        el-button(size="small" v-show="scope.row.enabled" @click="changeMaker(scope.row)") 不許可
    el-table-column(prop="deleted" label="削除" :filters="[{ text: '削除済み', value: true }, { text: '未削除', value: false }]" :filter-method="filterDeleted" filter-placement="bottom-end" width="120")
      template(scope="scope")
        span(v-text="scope.row.deleted?'削除済み':'－'")
</template>

<script>
export default {
  metaInfo: {
    title: 'メーカー管理',
  },
  data() {
    return {
      loading: false
    }
  },
  computed: {
    tableData() {
      return this.$store.getters['maintenance/getMakerList']
    }
  },
  methods: {
    filterEnabled(value, row) {
      return row.enabled === value
    },
    filterDeleted(value, row) {
      return row.deleted === value
    },
    changeMaker(maker) {
      this.loading = true
      maker.enabled = maker.enabled ? false : true
      this.$store.dispatch('maintenance/setMakers', maker).then(() => {
        this.$store.dispatch('maintenance/getMakers').then(() => { this.loading = false })
      })
    },
    getMaker() {
      this.loading = true
      this.$store.dispatch('maintenance/getMakers').then(() => { this.loading = false })
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
        { path: '/makers', name: 'メーカー' }
      )
    })
  },
  beforeRouteLeave(to, from, next) {
    // this.$store.commit('maintenance/setMakers', [])
    this.$store.dispatch('maintenance/clearMakers')
    next()
  }
}
</script>

<style lang="scss" scoped>
.fulltable {
  width: 100%;
}
</style>
