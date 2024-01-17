# data-gate
整合多个plc通信开源库.增加图形界面,以及上抛功能.目的是打造在生产环境中稳定可靠的plc采集程序.


/*
nodejs v20.10.0
npm init
npm install crawler
npm i --save lodash

https://github.com/bda-research/node-crawler?tab=readme-ov-file
https://github.com/cheeriojs/cheerio/wiki/Chinese-README#childrenselector
*/

const Crawler = require('crawler');
var _ = require('lodash');

// === 工具函数区 ===
function isDetailPage(pathname) {
  return (pathname != null && pathname.includes('list')) ? true : false;
}
// === 工具函数区 ===
var baseUrl = 'https://www.zgjm.org';
// === 数据结构定义 === 
/*
{
  renwu: {
    id: "renwu",
    name: "人物",
    url: ["https://www.zgjm.org/renwu/list_1.html", "https://www.zgjm.org/renwu/list_2.html"],
    items: [
      {
        id: "/renwu/tongshi",
        name: "同事",
        url: "https://www.zgjm.org/renwu/tongshi.html"
        desc: [
          {
            type: "传统",
            text: "梦见...."
          },
          {
            type: "心理学",
            text: "梦见...."
          }, {
            type: "通常",
            text: "梦见...."
          }
        ]
      }
    ]
  }
}

    
*/
// === 数据结构定义 === 
var data = {};
// === 分类树构建 === 
function parseCategory($, res) {
  // 1. 解析当前页面类型
  // 2. 抓取分类名称与url
  // 3. 构造分类地址,入队列
  // 4. 判断分页,并构造分页,入队列
  var category = $('h3').children().last();
  var name = category.text();
  var uri = res.request.uri.pathname;
  var href = category.attr('href');
  var cKey = _.split(href,'/')[1];
  console.log(`当前分类名称: ${name},分类Key: ${cKey},地址: ${uri}`);  
  var catObj = {
      id: cKey,
      name: name,
      url: [],
      items: []
  };
  if(data[cKey] ==undefined ||  Object.keys(data[cKey]).length === 0 ){
    data[cKey] = catObj;
  }else{
    catObj = data[cKey];
  }
  // 2. 抓取分类名称与url
  $('#list').children('ul').find('a').each(function(i,e){
    var je = $(this);
    var subName = je.text();
    var subHref = je.attr('href');
    console.log(`关键字=${subName}, upath=${subHref}`);
  });
}
// === 分类树构建 === 

// === 详情解析 === 
function parseDetail($, res) {

}
// === 详情解析 === 

const c = new Crawler({
  proxy: 'http://127.0.0.1:10809',
  jQuery: {
    name: 'cheerio',
    options: {
      normalizeWhitespace: true,
      xmlMode: true
    }
  },
  rateLimit: 1000,
  // This will be called for each crawled page
  callback: (error, res, done) => {
    if (error) {
      console.log(error);
    } else {
      console.log("请求地址: " + res.request.uri.pathname);
      const $ = res.$;
      // $ is Cheerio by default
      //a lean implementation of core jQuery designed specifically for the server
      // console.log($('title').text());
      // 区分 二级分类 与 详情页
      if (res.request.uri.pathname != null && res.request.uri.pathname.includes('list')) {
        // 构建二级分类树
        parseCategory($, res);
      } else {
        parseDetail($, res);
      }
    }
    done();
  }
});

var site = 'http://www.zgjm.org';
// Queue just one URL, with default callback
// c.queue('https://www.zgjm.org');
// document.querySelector("body > div.cont > div.col_left > div.mod_box_t2.zodiac_part > div.hd > h3 > a:nth-child(3)")
// Queue a list of URLs
c.queue([
'https://www.zgjm.org/renwu/list_1.html',
'https://www.zgjm.org/dongwu/list_1.html',
'https://www.zgjm.org/zhiwu/list_1.html',
'https://www.zgjm.org/wupin/list_1.html',
'https://www.zgjm.org/huodong/list_1.html',
'https://www.zgjm.org/shenghuo/list_1.html',
'https://www.zgjm.org/ziran/list_1.html',
'https://www.zgjm.org/guishen/list_1.html',
'https://www.zgjm.org/jianzhu/list_1.html',
'https://www.zgjm.org/qita/list_1.html'
]);
