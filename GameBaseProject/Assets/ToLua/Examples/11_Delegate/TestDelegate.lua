function DoClick1(go)                
                print('click1 gameObject is '..go.name)                    
            end

            function DoClick2(go)                
                print('click2 gameObject is '..go.name)                              
            end                       

            function AddClick1(listener)       
                if listener.onClick then
                    listener.onClick = listener.onClick + DoClick1                                                    
                else
                    listener.onClick = DoClick1                      
                end                
            end

            function AddClick2(listener)
                if listener.onClick then
                    listener.onClick = listener.onClick + DoClick2                      
                else
                    listener.onClick = DoClick2
                end                
            end

            function SetClick1(listener)
                if listener.onClick then
                    listener.onClick:Destroy()
                end

                listener.onClick = DoClick1         
            end

            function RemoveClick1(listener)
                if listener.onClick then
                    listener.onClick = listener.onClick - DoClick1      
                else
                    print('empty delegate')
                end
            end

            function RemoveClick2(listener)
                if listener.onClick then
                    listener.onClick = listener.onClick - DoClick2    
                else
                    print('empty delegate')                                
                end
            end

            --测试重载问题
            function TestOverride(listener)
                listener:SetOnFinished(TestEventListener.OnClick(DoClick1))
                listener:SetOnFinished(TestEventListener.VoidDelegate(DoClick2))
            end

            function TestEvent()
                print('this is a event')
            end

            function AddEvent(listener)
                listener.onClickEvent = listener.onClickEvent + TestEvent
            end

            function RemoveEvent(listener)
                listener.onClickEvent = listener.onClickEvent - TestEvent
            end

            local t = {name = 'byself'}

            function t:TestSelffunc()
                print('callback with self: '..self.name)
            end       

            function AddSelfClick(listener)
                if listener.onClick then
                    listener.onClick = listener.onClick + TestEventListener.OnClick(t.TestSelffunc, t)
                else
                    listener.onClick = TestEventListener.OnClick(t.TestSelffunc, t)
                end   
            end     

            function RemoveSelfClick(listener)
                if listener.onClick then
                    listener.onClick = listener.onClick - TestEventListener.OnClick(t.TestSelffunc, t)
                else
                    print('empty delegate')
                end   
            end