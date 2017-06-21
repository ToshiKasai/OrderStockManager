<template lang="pug">
div
    el-table.fulltable(:data="tableData" v-loading="loading" element-loading-text="Loading...")
        el-table-column.col2(prop="userName" label="コード" sortable)
        el-table-column.col2(prop="name" label="ユーザー名")
        el-table-column.col2(prop="expiration" label="有効期限" sortable)
            template(scope="scope")
                span(v-text="converetDateFormat(scope.row.expiration)")
        el-table-column.col3(prop="email" label="メールアドレス")
        el-table-column.col1(prop="deleted" label="削除" :filters="[{ text: '削除済み', value: true }, { text: '未削除', value: false }]" :filter-method="filterTag" filter-placement="bottom-end")
            template(scope="scope")
                span(v-text="scope.row.deleted?'削除済み':'－'")
        el-table-column(label="機能")
            template(scope="scope")
                el-button(size="smaill") ロール
                el-button(size="smaill") メーカー
</template>

<script>
// import { dateToFormatString } from '../../libraries/dateToFormatString';
import moment from 'moment';
moment.locale('ja', { weekdays: ["日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"], weekdaysShort: ["日", "月", "火", "水", "木", "金", "土"], })

export default {
    data() {
        return {
            loading: false
        }
    },
    computed: {
        tableData() {
            return this.$store.getters['maintenance/getUserList']
        }
    },
    beforeRouteEnter(to, from, next) {
        next(vm => {
            // `vm` を通じてコンポーネントインスタンスにアクセス
            vm.loading = true
            vm.$store.dispatch('maintenance/getUsers').then(() => { vm.loading = false })
        })
    },
    methods: {
        filterTag(value, row) {
            return row.deleted === value;
        },
        converetDateFormat(date) {
            // var tmp = new Date(date)
            // return dateToFormatString(tmp, '%YYYY%/%MM%/%DD%(%w%)')
            var tmp = moment(date)
            return tmp.format('YYYY/MM/DD(ddd)')
        }
    }
}
</script>

<style lang="scss" scoped>
.fulltable {
    width: 100%;
}
.col3 {
    width: 24%
}
.col2 {
    width: 16%
}
.col1 {
    width: 8%
}
</style>
