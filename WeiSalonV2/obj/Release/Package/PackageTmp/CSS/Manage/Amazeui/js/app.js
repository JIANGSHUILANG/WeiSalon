$(function(){
	//表单验证
	$('.am-form').validator();
	//复制
	$('.copy-link-btn').zclip({
	    path: '/CSS/Manage/Amazeui/js/ZeroClipboard.swf', //记得把ZeroClipboard.swf引入到项目中 
        copy:function(e){
        	var $this = $(e.currentTarget);
            return $this.prev('.copy-link').text();
        },
        afterCopy:function(e){/* 复制成功后的操作 */
        	var $this = $(e.currentTarget);
            var $tooltip = $('<div class="vld-tooltip">复制成功</div>');

 			$tooltip.css({left: $this.offset().left-30, top: $this.offset().top-40}).appendTo($('body')).fadeIn(200).delay(400).animate({'opacity':0},200,function(){
 				$(this).remove();
 			});
        }
    });
    $('.page-link .am-btn').zclip({
        path: '/CSS/Manage/Amazeui/js/ZeroClipboard.swf', //记得把ZeroClipboard.swf引入到项目中 
        copy:function(e){
        	var $this = $(e.currentTarget);
            return $this.parents('.page-link').eq(0).find('.am-form-field').val();
        },
        afterCopy:function(e){/* 复制成功后的操作 */
        	var $this = $(e.currentTarget);
            var $tooltip = $('<div class="vld-tooltip">复制成功</div>');

 			$tooltip.css({left: $this.offset().left-8, top: $this.offset().top-40}).appendTo($('body')).fadeIn(200).delay(400).animate({'opacity':0},200,function(){
 				$(this).remove();
 			});
        }
    });


    $('.menu_item a').each(function(){
        var _this = $(this);
        var href = _this.attr('href');
        if(location.pathname.indexOf(href) !== -1){
            _this.addClass('active');
        }
    });

    $('#statistics').on('open.collapse.amui', function(e) {
        //图表展示dom
        var $panel = $(e.target).show().find('.am-panel-bd');

        if(!$panel.attr('_echarts_instance_')){
          
            $.ajax('/Manage/GetgraphRealTimeStatistics',{data: {id: $panel.parent().data('id')}, type: 'post' ,dataType: 'json'}).done(function(res){
                res = JSON.parse(res);
                if (res.status != 1) {
                    alert(res.msg);
                    return false;
                }

                var aMonth = [], aOrder = [], aMoney = [];
                var date = 0, dateitem = '';
                for (var i = 29; i >= 0; i--) {
                    oDate = new Date() - i * 1000 * 60 * 60 * 24;
                    oDate = new Date(oDate);
                    dateitem = oDate.getMonth() + 1 + '月' + oDate.getDate() + '日';
                    aMonth.push(dateitem);

                    if (res.info[dateitem]) {
                        aOrder.push(res.info[dateitem].all_count);
                        //aMoney.push(res.info[dateitem].all_price);
                    }
                    else {
                        aOrder.push(0);
                        //aMoney.push(0);
                    }
                };

                // 基于准备好的dom，初始化echarts图表
                var oChart = echarts.init($panel[0], 'macarons');
                var option = {
                    title: {
                        text: '最近30天订单情况',
                        x: 'center'
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: ['预约单数'],
                        y: '30'
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            boundaryGap: false,
                            axisLine: {
                                lineStyle: {
                                    width: 1
                                }
                            },
                            data: aMonth
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value',
                            axisLine: {
                                lineStyle: {
                                    width: 1
                                }
                            },
                            axisLabel: {
                                formatter: '{value} 单'
                            }
                        }
                    ],
                    series: [
                        {
                            name: '预约单数',
                            type: 'line',
                            data: aOrder
                        }
                    ]
                };
                // 为echarts对象加载数据 
                oChart.setOption(option);
            });
        }
    });
    
    //查询数据
    (function(){
        var $alert = $('#date-alert');
        var iStart = new Date($('#date-start').val());
        var iStop = new Date($('#date-end').val());
        $('#date-start').datepicker().on('changeDate.datepicker.amui', function(event) {
            iStart = new Date(event.date);
            $(this).datepicker('close');
        });
        $('#date-end').datepicker().on('changeDate.datepicker.amui', function(event) {
            if(iStart>0 && event.date.valueOf()<iStart.valueOf()){
                $alert.text('结束日期应大于开始日期！').show();
            }
            else{
                $alert.hide();
            }
            iStop = new Date(event.date);
            $(this).datepicker('close');
        });
        $('#search-data-btn').on('click',function(){
            if(iStart===0 || iStop===0){
                $alert.text('开始结束日期不能为空！').show();
                return false;
            }
            if(iStop.valueOf()<iStart.valueOf()){
                $alert.text('结束日期应大于开始日期！').show();
                return false;
            }
            $alert.hide();
            //提交表单
        });
    })();
});