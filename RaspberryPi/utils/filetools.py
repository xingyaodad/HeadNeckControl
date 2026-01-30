import math

def lerp(p1,p2,v):
    return (p2 - p1) * v + p1

#支持 -1 到 1
def lerp2(p1,p2,v):
    return (p2 - p1) * (v * 0.5 + 0.5) + p1

#支持 -1 到 1，并且从自定义中间数值过渡
def lerp3(p1,p0,p2,v):
    if v <=0 :
        return lerp(p0,p1,-v)
    return lerp(p0,p2,v)

def doubleLerp(array1,array2,v):
    v1 = lerp(array1[0],array1[1],v)
    v2 = lerp(array2[0],array2[1],v)
    return [v1,v2]

def ease_in_out(t):
    # t ∈ [0,1] → 返回平滑过渡曲线值
    return t * t * (3 - 2 * t)

#更平滑
def smootherstep(t):
    return t**3 * (t * (t * 6 - 15) + 10)


def remap(n, inMin, inMax):
    t = (n - inMin) / (inMax - inMin)
    if t < 0:
        t = 0
    if t > 1:
        t = 1
    return t

def clamp(min_v,max_v,v):
    if v < min_v:
        return min_v
    if v > max_v:
        return max_v
    return v


def quaternion_to_euler(x, y, z, w):
    roll2 = 0
    pitch = 0
    yaw = 0
    # rool --X
    sinr_cosp = 2.0 * (w * x + y * z)
    cosr_cosp = 1.0 - 2.0 * (x * x + y * y)
    roll2 = math.atan2(sinr_cosp, cosr_cosp)
    # pitch --Y
    sinp = 2.0 * (w * y - z * x)
    if math.fabs(sinp) >= 1:
        pitch = math.copysign(math.pi / 2, sinp)
    else:
        pitch = math.asin(sinp)
    # yaw --Z
    siny_cosp = 2.0 * (w * z + x * y)
    cosy_cosp = 1.0 - 2.0 * (y * y + z * z)
    yaw = math.atan2(siny_cosp, cosy_cosp)
    return [math.degrees(roll2), math.degrees(pitch), math.degrees(yaw)]
