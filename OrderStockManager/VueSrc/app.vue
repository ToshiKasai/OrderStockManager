<template lang="pug">
div(v-loading="nowLoadingFull" :element-loading-text="loadingMessage")
  el-menu(mode="horizontal" :router="false")
    el-menu-item(index="homePath" v-text="title" @click="goHome")
    el-menu-item.header-menu(index="signin" v-show="!isAuthenticated" @click="signin") サインイン
    el-submenu.header-menu(index="acount" v-show="isAuthenticated")
      template(slot="title") {{fullName}}
      el-menu-item(index="config" @click="config") 設定変更
      el-menu-item(index="sinout" @click="signout") サインアウト
  #content(v-loading="nowLoading" :element-loading-text="loadingMessage")
    el-row(style="padding-bottom:8px;")
      el-col.bread(:span="24")
        el-breadcrumb(separator="/")
          el-breadcrumb-item(v-for="(item,index) in breadlist" :to="{path: item.path}")
            i.material-icons(v-show="index===0" style="font-size:13px;") &#xE88A;
            |{{item.name}}
    router-view
  footer#footer
    |Copyright&copy; 2016,2017 -
    a(href="https://github.com/minotaka" target="new") Minoru Takayama
    |All Rights Reserved.
</template>

<script>
import { generateUUID } from './libraries/generateUUID'
export default {
  metaInfo: {
    title: '',
    titleTemplate: '%s - 発注在庫管理 for arcane'
  },
  data() {
    return {
      title: "発注在庫管理 for arcane"
    }
  },
  computed: {
    isAuthenticated: function () {
      return this.$store.getters.isAuthenticated
    },
    fullName: function () {
      return this.$store.getters['fullName']
    },
    breadlist: function () {
      return this.$store.getters.getBreadlist
    },
    nowLoadingFull: function () {
      return this.$store.getters.nowLoadingFull
    },
    nowLoading: function () {
      return this.$store.getters.nowLoading
    },
    loadingMessage: function () {
      return this.$store.getters.loadingMessage
    }
  },
  methods: {
    goHome() {
      this.$store.dispatch('endLoading')
      //if (this.$store.getters.isAuthenticated) {
      //  this.$router.push('/menu')
      //} else {
        this.$router.push('/')
      //}
    },
    config() {
      this.$router.push('/config')
    },
    signin() {
      this.$router.push('/signin')
    },
    signout() {
      this.$store.dispatch('nowLoadingFull','サインアウト中...')
      this.$store.dispatch('signOut').then(() => {
        this.$store.dispatch('endLoading')
        this.$router.push('/')
      })
    }
  },
  created: function () {
    if (!this.$store.getters.isAuthenticated) {
      this.$ls.set("bearer", null)
      this.$ls.set("refresh", null)
    }
    if (this.$ls.get("clientId", null) == null) {
      this.$ls.set("clientId", generateUUID())
    }
  }
}
</script>

<style lang="scss" scoped>
#content {
  margin: 0 0 25px 0;
  padding: 8px 16px 8px 16px;
}

#footer {
  position: fixed;
  bottom: 0;
  width: 100%;
  height: 25px;
  background-color: lightgray;
  text-align: center;
  vertical-align: central;
}

.bread {
  background-color: #EFF2F7;
  border-radius: 4px 4px 0 0;
  padding: 8px 0 8px 4px;
}

.header-menu {
  float: right!important;
  margin-right: 16px;
  padding-right: 16px;
}
</style>
