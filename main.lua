local Camera = require 'camera'
--local Vector2 = require 'vector'

local Skills = {}

function NewSkill(ID,name,textSize,x,y,w,h,connections,color)
    local Skill = {
        ID=ID,
        name=name,
        textSize=textSize,
        x=(x-w/2),y=(y-h/2),
        cnx=x,cny=y,
        w=w,h=h,
        connections=connections,
        color=color
    }
    table.insert(Skills,Skill)
    return Skill
end
local gameIsPaused = false

local GameWidth, GameHeight = love.graphics.getDimensions()
local cam = Camera.new(0,0)
local PoppinsSBFont


function love.load()
    NewSkill(
        0,"Fundamental Gameplay Elements",13,
        -426,0,150,50,
        nil,{230,25,25}
    )

    NewSkill(
        100,"Consistency",20,
        0,0,150,50,
        nil,{0,0,255}
    )
    PoppinsSBFont = love.graphics.newFont("Poppins-SemiBold.ttf", 15)
    print(Skills[1].cnx,Skills[2].cnx,Skills[2].x)
end

local ReleasedMousePos = {0,0}
CursorX,CursorY = 0,0
CamX,CamY = 0,0
Scrolldist= 0
local function dump(o)
    if type(o) == 'table' then
       local s = '{ '
       for k,v in pairs(o) do
          if type(k) ~= 'number' then k = '"'..k..'"' end
          s = s .. '['..k..'] = ' .. dump(v) .. ','
       end
       return s .. '} '
    else
       return tostring(o)
    end
 end

function love.update(dt)
    collectgarbage("collect")
    local camZoom = cam.scale
    OldCX,OldCY = CursorX,CursorY
    CursorX,CursorY = love.mouse.getPosition()
    CamX,CamY = cam:position()
    if love.mouse.isDown(1) then
        cam:move((OldCX-CursorX)/camZoom,(OldCY-CursorY)/camZoom)
    end
end

function love.wheelmoved(x, y)
    if y > 0 then
        if Scrolldist < 1 then
            Scrolldist = 0.8
        end
        Scrolldist=Scrolldist+ 1/5
        if Scrolldist >= 2 then
            Scrolldist=Scrolldist+ 1/5
            return
        end
    else
        if Scrolldist > 1 then
            Scrolldist = 1
        end
        Scrolldist =  Scrolldist * 0.8
        if cam.scale < 0.5 then
            Scrolldist =  Scrolldist * 0.8
            return
        end
    end
    cam:zoom(Scrolldist)
end

function love.draw()

    -- [Background Layer] --



    -- [Camera Layer] --
    cam:attach()

    if gameIsPaused then
        love.graphics.print("=)", 400, 300)
    end

        -- Draw SkillBoxes
    for i,Skill in ipairs(Skills) do
        -- Box
        love.graphics.setColor(255,255,255)
        love.graphics.rectangle("fill",Skill.cnx,Skill.cny,Skill.w,Skill.h)
        love.graphics.setColor(0,0,0)

        -- Text
        PoppinsSBFont = love.graphics.newFont("Poppins-SemiBold.ttf", Skill.textSize)
        love.graphics.setFont(PoppinsSBFont)
        local width,lines = PoppinsSBFont:getWrap(Skill.name,Skill.w)
        local Fheight = PoppinsSBFont:getHeight(Skill.name)
        love.graphics.printf(Skill.name,Skill.cnx,Skill.cny+Skill.yoffset,Skill.w,"center") -- TODO: auto-center the text's Y Position based on the skill scaling
        --print(Skill.name,Skill.y,Skill.h,"|",Skill.textSize)
        love.graphics.print(tostring(Skill.cnx),Skill.cnx,Skill.cny)
        love.graphics.setColor(255,0,0)
        love.graphics.rectangle("fill",Skill.cnx,Skill.cny,10,10)
    end


    love.graphics.setColor(0,255,0)
    love.graphics.rectangle("fill",0,0,10,10)

    cam:detach()
    -- [HUD Layer] --

    PoppinsSBFont = love.graphics.newFont("Poppins-SemiBold.ttf", 18)
    love.graphics.setFont(PoppinsSBFont)
    love.graphics.print("CursorX "..tostring(CursorX)..", CursorY "..tostring(CursorY), 0, 0)
    love.graphics.print("CamX "..tostring(CamX)..", CamY "..tostring(CamY), 0, 15)
    love.graphics.print("CursorCamX "..tostring(CamX+CursorX)..", CursorCamY "..tostring(CamY+CursorY), 0, 30)
    love.graphics.print("Scroll: "..tostring(Scrolldist), 0, 45)

end

function love.focus(f)
    gameIsPaused = not f
end