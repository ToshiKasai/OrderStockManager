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
  metaInfo: function () {
    return {
      title: this.title
    }
  },
  data() {
    return {
      title: 'ユーザーメーカー',
      user: {},
      makers: [],
      myMakers: [],
      makerList: []
    }
  },
  methods: {
    getUser() {
      return this.$store.dispatch('maintenance/getUser', this.id).then((response) => {
        var item = response.data
        this.user = this.minotaka.makeSingleType(item, "object")
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
      })
    },
    getMakers() {
      return this.$store.dispatch('maintenance/getMakerList').then((response) => {
        var items = this.minotaka.makeArray(response.data)
        this.makerList = Enumerable.from(items).orderBy(x => x.code).toArray()
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
      })
    },
    getUserMakers() {
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
      })
    },
    getUserEditData() {
      this.$store.dispatch('nowLoadingMainte', 'ユーザー情報処理中')
      this.getUser()
      this.getMakers().then(() => {
        this.getUserMakers().then(() => {
          this.$store.dispatch('endLoading')
        })
      })
    },
    submitForm() {
      this.$store.dispatch('nowLoadingMainte', 'ユーザー情報更新中')
      this.$store.dispatch('maintenance/setUserMakers', { id: this.id, makers: this.makers }).then((response) => {
        this.$notify({ title: '変更完了', message: 'ユーザー情報の更新を行いました' })
        this.$store.dispatch('endLoading')
        this.$router.go(-1)
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.$store.dispatch('endLoading')
      })
    },
    cancel() {
      this.$router.go(-1)
    },
    selectChange(sels) {
      this.makers = sels;
    }
  },
  created() {
    this.getUserEditData()
  },
  watch: {
    '$route': 'getUserEditData'
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
.basemargin {
  margin: 8px
}

.full {
  width: 100%
}
</style>
