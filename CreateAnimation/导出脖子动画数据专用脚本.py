import maya.cmds as cmds
import math
import os

class ImportUI:
    def __init__(self):
        self.create_ui()

    def create_ui(self):
        if cmds.window('OutAnimWin', exists=True):
            cmds.deleteUI('OutAnimWin')

        self.win = cmds.window('OutAnimWin', title='导出动画数据', widthHeight=(320, 570), sizeable=False)
        main_layout = cmds.columnLayout(adjustableColumn=True, columnAttach=('both', 10))

        cmds.separator(height=10, style='in', parent=main_layout)
        frame_layout = cmds.flowLayout(parent=main_layout)
        cmds.text(label='开始和结束帧：', parent=frame_layout)
        cmds.intField('startFrame', parent=frame_layout,w = 100, value=cmds.playbackOptions(q=True, min=True))
        cmds.intField('endFrame', parent=frame_layout, w = 100, value=cmds.playbackOptions(q=True, max=True))
        
        cmds.separator(height=10, style='in', parent=main_layout)
        frame_layout = cmds.flowLayout(parent=main_layout)
        cmds.text(label='脖子骨骼名字：', parent=frame_layout)
        cmds.textField('jointName', parent=frame_layout, w = 100, text = 'neck')
        cmds.separator(height=10, style='in', parent=main_layout)
        cmds.button(label="导出当前动画为 .anim", p=main_layout, h=40, w=300, c=exportFile)
        
        cmds.showWindow(self.win)

def exportFile(*args):
    startFrame = cmds.playbackOptions(q=True, min=True)
    endFrame = cmds.playbackOptions(q=True, max=True)
    jointName = cmds.textField('jointName',q=True, text = True)
    filePath = cmds.fileDialog2(fileMode=0, caption="保存为 .anim 文件", okCaption="保存", fileFilter="*.anim")
    if not filePath:
        print('取消导出')
        return

    filePath = filePath[0]
    dataList = []

    dataList.append('1')

    for frame in range(int(startFrame), int(endFrame)+1):
        cmds.currentTime(frame)
        rot = cmds.getAttr(jointName + '.rotate')[0]
        line = ' '.join(['%.6f' % r for r in rot])
        dataList.append(line)

    # 写入文件
    with open(filePath, 'w') as f:
        f.write('\n'.join(dataList))

    print("成功导出数据到文件：%s" % filePath)

if __name__ == '__main__':
    ImportUI()
