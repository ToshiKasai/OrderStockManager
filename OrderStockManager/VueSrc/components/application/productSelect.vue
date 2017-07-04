<template lang="pug">
div
  el-form(label-width="150px" :inline="true" )
    el-form-item(label="選択メーカー")
      |{{selectMaker.code}} : {{selectMaker.name}}
    el-form-item(label="選択グループ")
      |{{selectGroup.code}} : {{selectGroup.name}}
    el-form-item(label="年度")
      |{{year}}

  el-card.box-card(v-for="data in selectViews" :id="'article-' + data.product.code")
    div {{data.product.code}} : {{data.product.name}}
    .subtitle ({{data.product.isSoldWeight?'計量品':'ピース品'}}／ケース：{{data.product.quantity}}／パレット：{{data.product.paletteQuantity | placeholder('未登録')}}／リードタイム：{{data.product.leadTime | placeholder('未登録')}})
    table.ptable(border="0")
      tr
        th(colspan="2") -
        th.datacell(v-for="o in 12") {{ (o + 8) % 12 + 1 }}月

      tr
        th.title-st(rowspan="2") 月初<br>在庫
        th.title-mid 予測
        td.datacell(v-for="o in 12") {{o * o}}
      tr
        th.title-mid 実績
        td(v-for="o in 12") {{data.salesList[o].zaiko_actual | currency('',data.product.isSoldWeight?3:0)}}

      tr
        th.title-st(rowspan="2") 発注
        th.title-mid 予定
          i.el-icon-edit
        td(v-for="o in 12")
          edit-number(:editval.sync="data.salesList[o].order_plan" :dispdlag="data.product.isSoldWeight" :results="recalc")
      tr
        th.title-mid 実績
        td(v-for="o in 12") {{data.salesList[o].order_actual | currency('',data.product.isSoldWeight?3:0)}}

      tr
        th.title-st(:rowspan="invoiceFlag?5:3") 入荷
        th.title-mid 予定
          i.el-icon-edit
        td(v-for="o in 12")
          edit-number(:editval.sync="data.salesList[o].invoice_plan" :dispdlag="data.product.isSoldWeight" :results="recalc")
      tr
        th.title-mid 実績
        td(v-for="o in 12") {{data.salesList[o].invoice_actual | currency('',data.product.isSoldWeight?3:0)}}
      tr
        th.title-mid(@click="toggleInvoice") 残数
          i.el-icon-arrow-down(:class="{'el-icon-arrow-up':invoiceFlag}")
        td(v-for="o in 12") {{data.salesList[o].invoice_zan - data.salesList[o].invoice_adjust | currency('',data.product.isSoldWeight?3:0)}}
      tr.hidden-title(v-show="invoiceFlag")
        th.title-mid 残数
        td(v-for="o in 12") {{data.salesList[o].invoice_zan | currency('',data.product.isSoldWeight?3:0)}}
      tr.hidden-title(v-show="invoiceFlag")
        th.title-mid 調整
          i.el-icon-edit
        td(v-for="o in 12" :class="{'text-danger':data.salesList[o].invoice_adjust<0}")
          edit-number(:editval.sync="data.salesList[o].invoice_adjust" :dispdlag="data.product.isSoldWeight" :results="recalc")
      tr
        th.title-st(rowspan="6") 販売
        th.title-mid 前年
        td(v-for="o in 12") {{data.salesList[o].pre_sales_actual | currency('',data.product.isSoldWeight?3:0)}}
      tr
        th.title-mid 予算
          i.el-icon-edit
        td(v-for="o in 12")
          edit-number(:editval.sync="data.salesList[o].sales_plan" :dispdlag="data.product.isSoldWeight" :results="recalc")
      tr
        th.title-mid 動向
        td(v-for="o in 12") {{data.salesList[o].sales_trend | currency('',data.product.isSoldWeight?3:0)}}
      tr
        th.title-mid 実績
        td(v-for="o in 12") {{data.salesList[o].sales_actual | currency('',data.product.isSoldWeight?3:0)}}
      tr
        th.title-mid 前年比
        td(v-for="o in 12") {{percentage(data.salesList[o].sales_actual,data.salesList[o].pre_sales_actual) | currency('',1)}}
      tr
        th.title-mid 予実比
        td(v-for="o in 12") {{percentage(data.salesList[o].sales_actual,data.salesList[o].sales_plan) | currency('',1)}}
  div#floadBtnMenu(@click="test")
    i.material-icons &#xE5D2;
  div#floatButton(@click="scrollTop")
    i.material-icons &#xE5D8;
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
      title: '商品データ一覧',
      groups: [],
      selectMaker: this.$store.getters.selectMaker,
      selectGroup: this.$store.getters.selectGroup,
      selectViews: [],
      year: new Date().getFullYear(),
      invoiceFlag: false
    }
  },
  methods: {
    getScreenData() {
      this.$store.dispatch('nowLoading', 'データ取得中')
      var promise
      if (this.selectGroup === null) {
        promise = this.$store.dispatch('getSalesviewsByMakerId', { id: this.selectMaker.id, year: this.year })
      } else {
        promise = this.$store.dispatch('getSalesviewsByGroupId', { id: this.selectGroup.id, year: this.year })
      }
      promise.then((value) => {
        this.selectViews = Enumerable.from(this.minotaka.makeArray(value.data)).orderBy(x => x.product.code).toArray()
        this.$store.dispatch('endLoading')
      }, (reasone) => {
        this.$store.dispatch('endLoading')
        this.$notify.error({ title: 'Error', message: reasone.message })
      })
    },
    groupSelect(row, event, column) {
      this.$store.commit('selectGroup', row)
    },
    percentage(param1, param2) {
      if (param2 === 0) {
        return 0
      } else {
        return param1 / param2 * 100;
      }
    },
    recalc(val) {
      console.log(val)
    },
    toggleInvoice() {
      this.invoiceFlag = this.invoiceFlag ? false : true
    },
    scrollTop() {
      var documentElement = null
      if (navigator.userAgent.toLowerCase().match(/webkit/)) {
        documentElement = document.body
      } else {
        documentElement = document.documentElement
      }
      documentElement.scrollTop = 0
      // window.scrollTop = 200
    },
    test() {
      // this.scrollTo('03210111')
      this.smoothScroll('article-03210111')
    },
    scrollTo(id) {
      var dat = "article-" + id
      var element = document.getElementById(dat)
      var rect = element.getBoundingClientRect()
      var pos = rect.top
      var documentElement = null
      if (navigator.userAgent.toLowerCase().match(/webkit/)) {
        documentElement = document.body
      } else {
        documentElement = document.documentElement
      }
      documentElement.scrollTop = pos + window.pageYOffset
    },
    smoothScroll(target) {
      // if (intervalID != null) return;
      // if (typeof target == 'string') target = document.querySelector(target);
      if (typeof target == 'string') target = document.getElementById(target);

      // 要素のY座標取得
      target.rect = target.getBoundingClientRect();
      target.posY = target.rect.top + window.pageYOffset;

      // スクロール方向
      var dir = (target.posY < window.pageYOffset) ? -1 : +1;
      // スクロール量
      var move = 20 * dir;
      // 合計スクロール量
      var totalScroll = window.pageYOffset;

      var intervalID = setInterval(function () {

        // 終了判定
        if ((dir == +1 && totalScroll >= target.posY) ||
          (dir == -1 && totalScroll <= target.posY)) {

          // 位置合わせ
          window.scrollTo(0, target.posY);

          clearInterval(intervalID);
          intervalID = null;
          return;
        }

        window.scrollBy(0, move);
        totalScroll += move;

      }, 10);
    }
  },
  created() {
    this.getScreenData()
  },
  watch: {
    '$route': 'getScreenData'
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      if (vm.$store.getters.selectMaker === null || vm.$store.getters.selectMaker.id === 0) {
        return '/'
      }
      vm.$store.commit('changeBreadcrumb',
        { path: vm.$route.path, name: vm.title }
      )
    })
  }
}
</script>

<style lang="scss" scoped>
.box-card {
  width: 100%;
  margin-bottom: 10px;
  font-size: 16px;
  overflow-x: scroll;
}

.subtitle {
  font-size: 14px;
}

$border-color: #E5E9F2; // Light Gray
$title-bgcolor: #D3DCE6; // Gray
$icon-color:#8492A6; // Silver
$hidden-bgcolor:#EFF2F7; // Extra Light Gray
$danger-color:#FF4949;
$warning-color:#F7BA2A;
.text-warning {
  color: $danger-color;
}

.bg-warning {
  background-color: #F7BA2A;
}

.text-danger {
  color: #FF4949;
}

.bg-danger {
  background-color: #FF4949;
}

table.ptable {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  text-align: left;
  line-height: 1.5;
  border-top: 1px solid $border-color;
  border-left: 1px solid $border-color;
}

table.ptable th {
  text-align: center;
  padding: 4px;
  font-weight: bold;
  vertical-align: top;
  border-right: 1px solid $border-color;
  border-bottom: 1px solid $border-color;
  border-top: 1px solid $border-color;
  border-left: 1px solid $border-color;
  background: $title-bgcolor;
  font-size: 14px;
  &.title-st {
    width: 4%;
    min-width: 30px;
  }
  &.title-mid {
    width: 6%;
    min-width: 50px;
  }
  &.datacell {
    width: 7%;
    min-width: 60px;
  }
}

.hidden-title {
  background-color: $hidden-bgcolor;
}

table.ptable td {
  text-align: right;
  padding: 4px;
  vertical-align: top;
  border-right: 1px solid $border-color;
  border-bottom: 1px solid $border-color;
  font-size: 14px;
}

i {
  margin-left: 8px;
  font-size: 10px;
  color: $icon-color;
}

#floadBtnMenu {
  position: fixed;
  left: 30px;
  top: 80px;
  i {
    font-size: 24px;
    color: #EFF2F7;
    width: 50px;
    height: 50px;
    margin: 0;
    border-radius: 50%;
    line-height: 50px;
    text-align: center;
    background-color: rgba(32, 160, 255, 0.2);
    transition-property: opacity, background, color;
    transition-duration: 0.5s;
    transition-timing-function: ease-in-out;
    &:hover {
      background-color: rgb(32, 160, 255);
    }
  }
}

#floatButton {
  position: fixed;
  right: 30px;
  bottom: 30px;
  i {
    font-size: 24px;
    color: #EFF2F7;
    width: 50px;
    height: 50px;
    margin: 0;
    border-radius: 50%;
    line-height: 50px;
    text-align: center;
    background-color: rgba(71, 86, 105, 0.2);
    transition-property: opacity, background, color;
    transition-duration: 0.5s;
    transition-timing-function: ease-in-out;
    &:hover {
      background-color: rgb(71, 86, 105);
    }
  }
}
</style>
