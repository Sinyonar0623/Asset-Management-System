import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar"
import { useParameter } from "@/context/ParameterContext"
import { LayoutDashboardIcon, LayoutListIcon, NewspaperIcon, SettingsIcon, ShelvingUnitIcon } from "lucide-react";

export function AppSidebar() {
  const { getGeneralCodeByGroup } = useParameter();
  const sidebarContent = getGeneralCodeByGroup("H001");

  const sidebarIcon = (t: string) => {
    switch (t) {
      case "01":
        return <LayoutDashboardIcon className="size-5"/>;
      case "02":
        return <LayoutListIcon/>;
      case "03":
        return <ShelvingUnitIcon/>;
      case "04":
        return <NewspaperIcon/>;
      case "05":
        return <SettingsIcon/>;
      default:
        break;
    }
  }

  return (
    <Sidebar collapsible="offcanvas" variant="floating">
      <SidebarHeader>

      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup className="space-y-3">
          {sidebarContent.map((context, index) => (
            <SidebarMenu key={index} className="">
              <SidebarMenuButton className="flex gap-3">
                <SidebarMenuItem>
                  {sidebarIcon(context.text)}
                </SidebarMenuItem>
                <SidebarMenuItem>
                  {context.text}
                </SidebarMenuItem>
              </SidebarMenuButton>
            </SidebarMenu>
          ))}
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
  )
}
