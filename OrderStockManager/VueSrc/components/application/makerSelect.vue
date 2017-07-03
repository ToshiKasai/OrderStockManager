<template lang="pug">
div
  el-form(label-width="150px")
    el-form-item(label="担当メーカーのみ")
      el-switch(v-model="selectMyMaker" on-text="" off-text="")
  el-table(ref="makerTable" :data="showData" height="480" stripe
    highlight-current-row @current-change="currentChange" @row-click="makerSelect")
    el-table-column(prop="code" label="コード" width="150")
    el-table-column(prop="name" label="名称")
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
      title: 'メーカー選択',
      makers: [],
      myMakers: [],
      selectMyMaker: this.$store.getters.selectMyMaker
    }
  },
  computed: {
    userId() {
      return this.$store.getters.userId
    },
    showData() {
      return this.selectMyMaker ? this.myMakers : this.makers
    }
  },
  methods: {
    getScreenData() {
      this.$store.dispatch('nowLoading', 'データ取得中')
      Promise.all([this.$store.dispatch('getMakers'), this.$store.dispatch('getMyMakers', this.userId)]).then((value) => {
        this.makers = Enumerable.from(this.minotaka.makeArray(value[0].data)).orderBy(x => x.code).toArray()
        this.myMakers = Enumerable.from(this.minotaka.makeArray(value[1].data)).orderBy(x => x.code).toArray()
        this.$store.dispatch('endLoading')
      }, (reasone) => {
        this.$store.dispatch('endLoading')
        this.$notify.error({ title: 'Error', message: reasone.message })
      })
    },
    setCurrent(id) {
      var row = Enumerable.from(this.minotaka.makeArray(this.showData)).select(x => x.id === id).toSingleOrDefault(null)
      this.$refs.makerTable.setCurrentRow(row);
    },
    currentChange(sel) {
      this.currentRow = sel;
    },
    makerSelect(row, event, column) {
      this.$store.commit('selectMaker', row)
      this.$router.push('/groupselect')
    },
    saveToggleMyMaker() {
      this.$store.commit('selectMyMaker', this.selectMyMaker)
    }
  },
  created() {
    this.getScreenData()
  },
  watch: {
    '$route': 'getScreenData',
    'selectMyMaker': 'saveToggleMyMaker'
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
