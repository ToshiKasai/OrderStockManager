<template lang="pug">
div
  el-button(@click="useradd") 新規登録
  el-table(:data="users" stripe height="480")
    el-table-column(prop="userName" label="コード" sortable width="150")
    el-table-column(prop="name" label="ユーザー名" min-width="200")
    el-table-column(prop="expiration" label="有効期限" sortable width="150")
      template(scope="scope")
        span {{scope.row.expiration | converetDateFormat}}
    el-table-column(prop="email" label="メールアドレス" min-width="300")
    el-table-column(prop="enabled" label="使用" :filters="enabledFilters" :filter-method="filterEnabled" filter-placement="bottom-end" :filter-multiple="false" width="100")
      template(scope="scope")
        span {{ scope.row.enabled | boolMessage('許可', '不可')}}
    el-table-column(prop="deleted" label="削除" :filters="disabledFilters" :filter-method="filterDisabled" filter-placement="bottom-end" :filter-multiple="false" width="100")
      template(scope="scope")
        span {{scope.row.deleted | deletedMessage}}
    el-table-column(label="機能" fixed="left" width="200")
      template(scope="scope")
        el-button(size="small" @click="edit(scope.row.id)") edit
        el-button(size="small" @click="role(scope.row.id)") role
        el-button(size="small" @click="maker(scope.row.id)") maker
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
      title: 'ユーザー管理',
      users: [],
      enabledFilters: [{ text: '許可', value: 'true' }, { text: '不可', value: 'false' }],
      disabledFilters: [{ text: '削除', value: 'true' }, { text: '未削除', value: 'false' }]
    }
  },
  methods: {
    filterEnabled(value, row) {
      return row.enabled.toString() === value
    },
    filterDisabled(value, row) {
      return row.deleted.toString() === value
    },
    getUsers() {
      this.$store.dispatch('nowLoadingMainte', 'ユーザー情報処理中')
      this.$store.dispatch('maintenance/getUsers').then((response) => {
        var items = response.data
        this.users = this.minotaka.makeArray(items)
        this.$store.dispatch('endLoading')
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.$store.dispatch('endLoading')
      })
    },
    edit(id) {
      this.$router.push({ name: 'useredit', params: { id: id } })
    },
    role(id) {
      this.$router.push({ name: 'userroles', params: { id: id } })
    },
    maker(id) {
      this.$router.push({ name: 'usermakers', params: { id: id } })
    },
    useradd() {
      this.$router.push({ name: 'useradd' })
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
