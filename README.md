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
const fse = require('fs-extra')

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
    url: ["https://www.zgjm.org/renwu/list_1.html","https://www.zgjm.org/renwu/list_2.html"],
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
  let pageUrl = baseUrl+uri; 
  console.log(`当前分类名称: ${name},分类Key: ${cKey},地址: ${pageUrl}`);  
  var catObj = {
      id: cKey,
      name: name,
      url: [pageUrl],
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
    let subPage = baseUrl+subHref;
    let subItem = {
      id: subHref,
      name: subName,
      url: subPage,
      desc: []
    };
    catObj.items.push(subItem);
    // console.log(`关键字=${subName}, upath=${subPage}`);
  });

  let subPages=[];
  // 获取次分类的分页信息
  $('.list-pages').children('ul').find('a').each(function(i,e){
    var je = $(this);
    var subName = je.text();
    var subHref = je.attr('href');
    let subPage = baseUrl+subHref;
    if(subName!='首页' && subName!='下一页'&& subName!='末页' && (_.indexOf(catObj.url, subPage)==-1)){
      // 排除第一页,加入后续采集队列
      console.log(`分页按钮: 按钮名=${subName}, 按钮uri=${subPage}`);
      catObj.url.push(subPage);
      subPages.push(subPage);      
    }
  });

  subPages = _.uniq(subPages);
  catObj.url = _.uniq(catObj.url);
  if(subPages.length>0){
    console.log(`待采集下一页链接: ${subPages}`);
    c.queue(subPages);      
  }else{
    console.log(`当前分类细项采集完成: 名称:${name},key:${cKey},子条目共计:`+catObj.items.length);
  }  
}
// === 分类树构建 === 

// === 详情解析 === 
function parseDetail($, res) {
  // $('div[class="hd"]').eq(0).children('h3').find('a') 表示
  let pe = $('div[class="hd"]').eq(0).children('h3').find('a');//.eq(2);
  let pname = pe.eq(2).text();
  let purl = pe.eq(2)
  let descName = pe.eq(3).text();
  let descUrl = pe.eq(3).attr('href');
  let pkey = _.split(purl,'/')[1]; // renwu
  let subKey = res.request.uri.pathname;
  let pObj = {};
  if(data[pkey] ==undefined ||  Object.keys(data[pkey]).length === 0 ){
    console.log(`当前项目没有找到主分类! 名称=${pname},key=${pkey}`);
    return;
  }else{
    pObj = data[cKey];
  }
  let pNode = _.find(pObj.items, function(o) {
    return o.age < 40; 
  });
  if(pNode==undefined){
    console.log(`当前项目没有二级分类,创建默认值! 名称=${pname},key=${pkey}`);
    pNode = {
      id: baseUrl+subKey,
      name: subName,
      url: subPage,
      desc: []
    };
  }
  
  $('div[class="read-content"]').children('p').slice(1).each(function(p){
    let descText = $(this).text();
    descText = _.trim(descText);
    if(descText.length<10){
      return ;
    }
    // 1 
    if(_.startsWith(descText,'原版周公解梦')){
      // 原版周公解梦
    }else if(_.startsWith(descText,'周易解梦')){
      // 周易解梦
    }else if(_.startsWith(descText,'心理学解梦')){
      // 周易解梦
    }
    console.log(descText);
  });
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
  rateLimit: 500,
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

// 队列已被清空
c.on('drain', () => {
    console.log('请求队列已完成! ');
    fse.writeJsonSync('data.json', data);
    // c.queue('https://www.zgjm.org/dongwu/list_1.html');
});

// Queue just one URL, with default callback
// c.queue('https://www.zgjm.org');
// document.querySelector("body > div.cont > div.col_left > div.mod_box_t2.zodiac_part > div.hd > h3 > a:nth-child(3)")
// Queue a list of URLs
/*c.queue([
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
]);*/
c.queue([
'https://www.zgjm.org/renwu/list_1.html'
]);
