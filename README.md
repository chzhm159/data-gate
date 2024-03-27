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




lot
小车码
port码


lot做之前,数量需要确认.
lot结束时,数量人工确认(成功,失败,剩余等数量)
module ID的校验功能需要预留,有可能是每扫一个,就校验一次

https://xie.infoq.cn/article/993ca07318588b8f2091500b1
https://taccisum.github.io/resume/

熟悉 Scrum 敏捷项目管理模式、DevOps、CI / CD，有团队管理经验，规范制定及落实经验


拥有10+年 开发经验，其中4年以上中型系统架构设计经验，以及5年以上的团队管理经验。技术扎实，广度深度均具备。具有高并发、高可用、大数据量的系统架构设计以及研发经验，包括从零到一的团队组建，研发流程、规范制定及落实，架构设计，迭代回顾会、技术分享会等关键制度或规范。目前管理10+人的研发团队，负责工业物联网平台的架构设计与研发。


专业技能
精通 Java，C#，NodeJS,使用过 C++，Python。
多线程、并发、AQS、集合，JVM、JMM、GC、调优等
其它语言，熟练 Node.js、V8、Egg.js、Nest.js，.NET、VB、Python、Lua 等等偏应用层和小工具开发，web 端有一些了解（Vue.js、Angular 等）
主流框架，Spring、Spring Boot、Spring Cloud 全家桶、Shiro、Mybatis、JPA 等基本都熟练
主流中间件，Redis、RabbitMQ / RocketMQ、ShardingJDBC 等运用、原理及常见问题处理方案
熟悉接入层、缓存、微服务、存储层三高方案，如主从、切片分库分表、负载均衡、故障转移、限流降级、分布式事务等等，可应对海量用户
熟悉大数据相关技术，如 Spark、Hadoop、Hdsf、Kafka 等
熟悉领域驱动设计，业务架构设计，服务的划分等有相关的方法和实践经验。熟悉设计原则、设计模式，能针对问题域设计出优雅的代码架构
熟悉新技术，如 Docker，Kubernetes 架构及原理，虚拟化技术、网络方案等，有实际经验
阅读过源码：Eureka、Zuul、Shiro 等


个人技能

1、精通 Java、设计模式、网络编程(Netty)、并发编程、JVM 内存管理及调优；

2、精通 spring. springMVC.Mybatis，阅读过相关源码并根据需要扩展；

3、精通 dubbo、spring cloud (Eureka、Ribbon、Feign、Hystrix、zuul)，阅读过相关源码；

4、精通分布式事务，阅读过 2PC、TCC 相关组件的源码，设计可靠消息最终一致性方案、最大努力通知方案、saga；

5、精通 Mysql，具有 sql 优化、Mycat 分库分表、"索引优化、性能调优、数据库灾备等丰富的实战经验；

6、精通 Redis，具有集群搭建(Twemproxy、Codis、Redis Cluster)，冷热备份，性能调优、数据迁移等实战经验；

7、熟练使用 MongoDB，具有集群搭建(副本集、分片集群)，冷热备份、数据迁移、安全管理等实战经验；

8、精通 ElasticSearch，熟练使用 Solr、Logstash、Beats 及 Kibana，搭建 ELK 日志收集；

9、消息中间件：掌握 activelMQ. rocketMQ.rabbitlMQ.kafka 原理及集群部署

10、负载均衡：熟练使用 Nginx (Iengine.Openresty) . Haproxy，LVS、KeepAlived、zookeeper 等负载均衡组件；

11、自动化部署:Git、Jenkins、Gradle；

12、容器化部署: docker，具有搭建 swarm、mesos、marathon、kubernetes 集群并运维经验；

13、自动化运维:熟练使用 Saltstack，数据监控(zabbix) ；

14、其他:了解并使用过 Hadoop、Hive、Strom 等大数据相关技术，使用过其他技术如 lua、Go、Python、规则引擎（drools）等；

15、建模工具:PowerDesigner,Rose、visio、等 UML 建模工具；

16、遵循华为测试规范，功能测试（单元测试、冒烟测试、集成测试、QA 测试)性能则试(Jmeter.LoadRunner)、自动化测试(selenium.QTP) ；

17、遵循华为安全体系，代码安全、加密安全、设计安全；

18、遵循华为完善的层级文档规范；








1. 完成状态采集
   1.1 多plc通信
   1.2 多plc报警状态合并显示
   1.3 报警的存储,查询


项目管理：基于CPM管理理念，实施过多家企业数字化工厂项目，共计近200多条数字化产线；以及超50台自动化设备MES追溯系统。从客户需求调研，架构设计，任务规划，关键问题攻克，项目实施，至验收。得到客户高度认可。

架构设计：
1. 负责制定整体系统架构及复杂业务的解决方案架构，并产出架构设计文档。
2. 负责平台基础搭建：使用数据库（MySQL，Postgresql，SQLServer）+ Elastic + InfluxDB， Nginx，Redis，RabbitMQ。 稳定应对 80+ 条自动化产线 7 X 24 小时生产数据的采集，存储，统计需求。
3. 负责平台演进规划： “以需求养平台”，真实项目中提炼需求，制定平台改进计划。 成功将平台从最初仅采集基础数据，演进到涵盖：排产计划（人）、设备维护（机）、产品/物料管理（料）、工艺管理（法）、环境监测（环）、产品测试（测）各个环节的工业数字化平台，并成功实施到2家，超过100条自动化产线的大型外资企业中。
4. 数据可视化：


采集网关：专注于解决工业物联网场景下，协议复杂，需求多变，传统采集网关授权费用高，功能受限，与现有技术对接难等问题。自主开发采集网关，基于异步IO模型（Netty），实现了 基恩士UpLink协议，欧姆龙Fins（TCP/UDP）协议，西门子 S7协议，Modbus/TCP等通信协议。 同时支持MQTT、AMQP、消息队列的上抛，Redis数据缓存，异常邮件、短信报警等功能。相对其它采集网关具有如下优势： 
1. 高并发，可扩展，稳定可控，基于异步IO，单机可以稳定支持超过100个PLC.
2. 小到嵌入上位机程序中使用，大到承担整车间数据采集服务，以及多机冗余功能都支持。
3. 基于TimingWheel算法，可毫秒级精准控制每个点位采集频率，且支持过期补偿，失败重试等特性。
4. 内置多种主动上抛策略，当值变动时上抛，当超出上下限时上抛，当值变动幅度超过一定百分比时上抛等等，可大幅简化应用端工作量和数据存储量。
5. 可扩展，可支持各种传感器，视觉控制器，板卡等设备。

1. 存储：关系型数据库+时序数据库， 应对工业场景下，数据关联查询多，数据统计功能多，数据写入频繁且量大的问题。
2. 定时服务： 基于 Quartz +SpringBoot 实现独立的定时服务。
3. 多维度数据统计框架：基于海量秒级原始记录，实现分钟级，小时级，班次级，天，周，月度等数据统计功能。


功能规划：根据需求设计系统功能，支持多种 PLC 通信协议的采集程序；人、机、料、法、环、测等相关功能；例如产量监控，产品不良数据，设备运行状态监控，人员排班管理，物料管理，环境监测，测量结果管控等等。实现综合稼动率（OEE），不良率（DPPM），设备稳定性指标（MTBF、MTTR），不良分类帕累托图，X-R均差图等SPC过程控制相关统计。

生产追溯系统：对接自动化产线，实现生产数据的采集，存储，预测。数据来源包括PLC，扫码枪，各种传感器，工业视觉控制器，测量板卡等等硬件设备。将产品的生产过程数据进行记录存储。











windows

技术难点:
   . 2台工控机,数据库中数据如何同步? 因为可能存在只开启一台的情况,处理方案
   . 局域网文件变动监控功能



待开发:
   
   . 程序中全局变量的缓存功能,
   . 使用SkiaSharp实现动画
   . 
   . 局部功能的屏蔽功能,例如扫码枪
   . 两台工控机的心跳功能实现(借助SECS/GEM 的代理实现.即可)

开发中:
   . 上抛数据资料整理
   . 搭建界面的初步设计
   . 多个plc链接管理
   . tag数据缓存架构设计
   . 中英文切换

基于SkiaSharp,实现一个元素绘制框架,可以增加子元素,并且具备zIndex属性,表示子元素的层级,可以对元素进行 平移,旋转,缩放,倾斜. 子元素同样跟着变换,子元素可以处理鼠标事件,但仅是zIndex最高的能响应事件.

4. 可以高亮元素边缘且颜色可配置
2. : 鼠标点击,鼠标悬停

讨论问题: 
1. 组装端 与 拆解端 两个上位机的功能有啥要求? 是否独立开发? 


2. 2台工控机,必须有一台始终开机.


2. 设备状态的讨论
   1. 运行中
   2. 停止中
   3. 报警中
   4. 急停中
   6. 待料中
   7. 工程模式中
   8. 待初始化
   9. 初始化中
3. 

248, 248, 248

设备模式:
   1. 全自动模式

已完成:
   .
   .   
   .
1. 手动模式下,完全不上抛数据,切换为现有人工模式
2. 



主色调(RAL 5023): 66, 105, 140
背景色(RAL 9003): 236, 236, 231
边框颜色(RAL 5012) : 0, 137, 182

绿色(RAL 6038): 0, 181, 26
红色(RAL 3011): 187, 30, 16
黄色(RAL 1026): 255, 255, 0


按钮-常规: 
  默认颜色(RAL 5023): 66, 105, 140
  鼠标移上去(RAL 5012): 0, 137, 182
  鼠标按下去(RAL 5022): 34, 45, 90

按钮-绿色
  默认颜色(RAL 6038): 0, 181, 26  
  鼠标移上去(RAL 5012): 0, 137, 182
  鼠标按下去(RAL 5022): 34, 45, 90  

颜色:https://rgb.to/ral/5023
(https://www.ralcolorchart.com/ral-classic/blue-hues)
白色: RAL 9003  相关: https://www.ralcolorchart.com/ral-classic/white-and-black-hues
蓝色: RAL 5023 相关:https://www.ralcolorchart.com/ral-classic/blue-hues



1. SEMI 协议版本
2. 如果扫码失效的情况下,需要弹窗,人工选择如何处理,例如整个lot 更换.或者人工纠正个别产品后处理
3. Lot 结束的判断标准是什么有待商榷
4. 如果部分手动的情况下,SECS/GEM 通信可能恢复部分功能,或者回退到原始状态
5. 设备运行状态定义可以参考Tube设备
6. 设备独立部分的运行状态,与整体的状态如何定义后续在讨论
7. 烘箱完成之后的需要读取一个数据文件,然后通过secs/gem协议上传
   7.1. 三个数据: 产品,jig数据,烘箱数据
8. 烘箱数据主要有2个,一个烘烤完之后数据汇总参考上文.一个是报警信息(实时监控).



软件设计阶段:
   1. PC端软件需求资料整理            2023/11/13,2023/11/17,  5天
   2. SECS/GEM数据,设备端数据方案设计  2023/11/18,2023/11/24,  7天
   3. 软件(PC端)界面,功能架构设计      2023/11/25,2023/12/01,  7天
   4. 软件功能优化与设计定稿           2023/12/02,2023/12/08,  7天


功能开发阶段:
   1. 基础框架搭建                   2023/12/09,2023/12/18 10天
   2. 与设备端数据交互功能开发        2023/12/19,2024/01/02 15天
   3. SECS/GEM相关功能开发           2024/01/03,2024/01/10 10天
   5. Jig寿命管控                    2024/01/11,2024/01/15 5天
   4. 生产历史本地存储查询开发        2024/01/16,2024/01/23 7天                   
   5. 数据统计功能开发               2024/01/24,2024/01/30 7天  

功能调试阶段:
   1. 调试&功能优化                  按项目时间计划即可


10天
15天
10天
5天
7天
7天  


1. 上料端上位机程序,下料端上位机程序,允许单独运行(即独立程序)                             15 人/天
2. SECS/GEM 协议支持,完成Host端数据下发处理,采集数据上抛,运行事件上抛,报警上抛            15 人/天
3. 客户原有设备报警日志文件监控,实时上抛,运行数据文件监控解析,并通过SECS/GEM上抛           5人/天
4. 生产数据绑定,历史记录(产品历史,报警历史,运行状态记录)                                 10人/天
5. 运行指标统计,OEE,报警时长,MTBA等等                                                  15人/天
6. 夹具寿命管控       15人天                                                                                                      


数据库,日志等基础组件搭建
PLC,RS232等通信组件搭建


10 系统&软件调试  Program & software Debugging  2024/1/30   2024/2/18
11 带料调试  Debugging with materials and try run   2024/2/19   2024/2/25
