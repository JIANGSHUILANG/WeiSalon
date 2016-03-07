/*
 * jquery.scrollbar - v1.3.1 - 2014-11-19
 * Copyright (c) 2014 xiaozhu。
 */
(function($){
	$.fn.extend({
		scrollbar : function(fixed){//fixed是否固定滑块大小，可配合css实现各种滑块。

			var _self = this;
			var oContent = _self.html();
			var iCntH,iBarH,iBtnH,iSpace;
			var newTop = 0;
			var oInner = $('<div class="scroll-inner"></div>'),
				oCnt = $('<div class="scroll-cnt"></div>'),
				oBar = $('<div class="scroll-bar"></div>'),
				oBtn = $('<div class="scroll-btn"></div>');

			oBar.append(oBtn);
			oCnt.append(oContent);
			oInner.append(oCnt).append(oBar);
			_self.html('').append(oInner);

			if(!_self.hasClass('jq-scroll')){
				_self.addClass('jq-scroll');//统一格式便于检测是否已经执行插件
			}
			//内容不足时隐藏滚动条
			if(oCnt.height()<=oBar.height()) {
				oBar.hide();
				return false;
			}

			iSpace = parseInt(oBar.css('marginLeft')) || 0;
			
			oCnt.width(oInner.width()-oBar.width()-iSpace).height('auto');//重新定义内容宽度

			iCntH = oCnt.height();
			iBarH = oBar.height();

			if(fixed){
				oBtn.addClass('scroll-fixed');//固定滑块大小
			}
			else{
				oBtn.height(iBarH*iBarH/iCntH);//自适应滑块大小、此处inner高度与bar一致所以直接使用iBarH
			}

			iBtnH = oBtn.height();

			oBtn.on({
				'mousedown':function(e){
					var _this = $(this);
					var basicTop = parseInt(_this.css('top'));
					_this.data('Y',e.pageY);
					
					$(document).on('mousemove',function(ev){

						newTop = basicTop + ev.pageY - _this.data('Y');
						onScrollRun();

					}).mouseup(function(){
						$(document).off('mousemove');
					})
				}
			});

			oInner.on('mousewheel DOMMouseScroll', function(ev){
				
				//js中用event.wheelDelta值判定滚动方向，jquery中用event.originalEvent.wheelDelta
				var iScroll= ev.originalEvent.wheelDelta?ev.originalEvent.wheelDelta<0:ev.originalEvent.wheelDelta>0;
				
				//bDown真则向下滚动鼠标，假则向上滚
				if(iScroll){	
					
					onScrollRun(1);
				}
				else{

					onScrollRun(-1);
				}
				
				return false;
			});

			function onScrollRun(speed){
				if(speed){
					var basicTop = parseInt(oBtn.css('top'));
					newTop = basicTop + 20*speed;
				}
				
				if(newTop<0){
					newTop = 0;
				}
				else if(newTop>iBarH-iBtnH){
					newTop = iBarH-iBtnH;
				}
				oBtn.css({'top': newTop})//滚动滑块
				oCnt.css({'top': -newTop*(iCntH-iBarH)/(iBarH-iBtnH)})//根据滑块位置滚动内容区域
			}

		}
	})
	
})(jQuery);