<template lang="pug">
div
    el-row
        el-col(:xs="8" :sm="6" :md="4" :lg="3")
            el-menu(mode="vertical" theme="dark" :router="true")
                el-menu-item(index="") 管理メニュー
                el-menu-item(index="users") ユーザーメンテ
                el-menu-item(index="roles") ロールメンテ
        el-col(:xs="16" :sm="18" :md="20" :lg="21")
            router-view
</template>

<script>
export default {
}
</script>

<style lang="scss" scoped>
</style>
