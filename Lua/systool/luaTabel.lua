luaTabel = luaTabel or {}

function luaTabel.XY( x,y )
	x = x or 0
	y = y or 0
	return {x = x ,y = y}
end

function luaTabel.XYZ( x,y,z )
	x = x or 0
	y = y or 0
	z = z or 0
	return {x = x ,y = y,z = z}
end