<template lang="pug">
div.basemargin
  el-row
    el-col(:span="2") 編集対象
    el-col(:span="22" v-text="user.name")
  el-row
    el-col(:span="12")
      el-table(:data="makers" style="width: 100%" height="400")
        el-table-column(label="担当メーカー")
          el-table-column(property="code" label="コード" width="120")
          el-table-column(property="name" label="メーカー名")
    el-col(:span="12")
      el-table(ref="multipleTable" :data="makerList" style="width: 100%" height="400" @selection-change="selectChange")
        el-table-column(label="メーカー一覧")
          el-table-column(type="selection" width="55")
          el-table-column(property="code" label="コード" width="120")
          el-table-column(property="name" label="メーカー名")
  el-button(type="primary" @click="submitForm") 変更
  el-button(@click="cancel") キャンセル
</template>

<script>
import Enumerable from 'linq';

export default {
  props: ['id'],
  metaInfo: {
    title: 'ユーザー権限',
  },
  data() {
    return {
      loading: false,
      user: {},
      makers: [],
      myMakers: [],
      makerList: []
    }
  },
  methods: {
    getUser() {
      this.loading = true
      return this.$store.dispatch('maintenance/getUser', this.id).then((response) => {
        var item = response.data
        this.user = this.minotaka.makeSingleType(item, "object")
        this.loading = false
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
    },
    getMakers() {
      this.loading = true
      return this.$store.dispatch('maintenance/getMakerList').then((response) => {
        var items = this.minotaka.makeArray(response.data)
        this.makerList = Enumerable.from(items).orderBy(x => x.code).toArray()
        this.loading = false
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
    },
    getUserMakers() {
      this.loading = true
      return this.$store.dispatch('maintenance/getUserMakers', this.id).then((response) => {
        var items = response.data
        this.myMakers = this.minotaka.makeArray(items, "object")
        var makerId = Enumerable.from(this.myMakers).orderBy(x => x.id).select(x => x.id).toArray()
        this.makers = Enumerable.from(this.makerList).where(x => { return makerId.indexOf(x.id) >= 0 }).toArray()
        this.makers.forEach(row => {
          this.$refs.multipleTable.toggleRowSelection(row);
        })
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
    },
    submitForm() {
      this.loading = true
      this.$store.dispatch('maintenance/setUserMakers', { id: this.id, makers: this.makers }).then((response) => {
        this.$notify({ title: '変更完了', message: 'ユーザー情報の更新を行いました' })
        this.loading = false
        this.$router.go(-1)
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
    },
    cancel() {
      this.$router.go(-1)
    },
    selectChange(sels) {
      this.makers = sels;
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.getUser(),
        vm.getMakers().then(() => {
          vm.getUserMakers()
        })
      vm.$store.commit('changeBreadcrumb',
        { path: '/mainte/userroles/' + vm.id, name: 'ユーザー権限' }
      )
    })
  }
}
</script>

<style lang="scss" scoped>
.basemargin {
  margin: 8px
}

.full {
  width: 100%
}
</style>
