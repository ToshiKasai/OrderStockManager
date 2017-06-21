<template lang="pug">
div
    el-breadcrumb(separator="/")
        el-breadcrumb-item(:to="{ path: '/menu' }") MENU
        el-breadcrumb-item めにゅー

    el-row(:gutter="20")
        el-col(:span="8")
            el-button(size="large" style="width:100%") 在庫・販売状況確認
        el-col(:span="8")
            el-button(size="large" style="width:100%") 予算データアップロード
        el-col(:span="8")
            el-button(size="large" style="width:100%") パスワード変更
    el-row(:gutter="20")
        el-col(:span="8")
            el-button(size="large" style="width:100%") メールアドレス変更
        el-col(:span="8")
            el-button(size="large" style="width:100%" @click="goMainte") 管理機能
        el-col(:span="8")
            el-button(size="large" style="width:100%") サインインログ表示
    el-row(:gutter="20")
        el-col(:span="8")
            el-button(size="large" style="width:100%") アクションログ表示
</template>

<script>
export default {
    methods: {
        goMainte() {
            this.$router.push('/mainte')
        }
    }
}
</script>

<style lang="scss" scoped>
</style>
