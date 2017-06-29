<template lang="pug">
div.basemargin
  el-row
    el-col(:span="2") 編集対象
    el-col(:span="22" v-text="user.name")
  el-transfer(v-model="roles" :data="roleList" :button-texts="['剥奪', '付与']" :titles="['未付与権限', '付与済み']" :props="{key: 'name', label: 'name'}")
  el-button(type="primary" @click="submitForm") 変更
  el-button(@click="cancel") キャンセル
</template>

<script>
export default {
  props: ['id'],
  metaInfo: {
    title: 'ユーザー権限',
  },
  data() {
    return {
      loading: false,
      user: {},
      roles: [],
      roleList: []
    }
  },
  methods: {
    getUser() {
      this.loading = true
      this.$store.dispatch('maintenance/getUser', this.id).then((response) => {
        var item = response.data
        this.user = this.minotaka.makeSingleType(item, "object")
        this.loading = false
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
    },
    getUserRoles() {
      this.loading = true
      this.$store.dispatch('maintenance/getUserRoles', this.id).then((response) => {
        var items = response.data
        this.roles = this.minotaka.makeArray(items, "string")
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
    },
    getRoles() {
      this.loading = true
      this.roleList = []
      this.$store.dispatch('maintenance/getRoleList').then((response) => {
        var items = this.minotaka.makeArray(response.data)
        this.roleList = items
        this.loading = false
      }).catch((error) => {
        this.$notify.error({ title: 'Error', message: error.message })
        this.loading = false
      })
    },
    submitForm() {
      this.loading = true
      this.$store.dispatch('maintenance/setUserRoles', { id: this.id, roles: this.roles }).then((response) => {
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
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.getUser(),
        vm.getRoles(),
        vm.getUserRoles(),
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
</style>
