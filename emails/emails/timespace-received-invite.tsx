import {
	Body,
	Button,
	Container,
	Head,
	Html,
	Img,
	Link,
	Preview,
	Section,
	Text,
  } from "@react-email/components";
  import * as React from "react";
import { colors } from "./colors";
  
  export const TimespaceConfirmEmailEmail = () => {
	return (
	  <Html>
		<Head />
		<Preview>Uitnodiging voor Timespace van {`{{ tenant_name }}`}</Preview>
		<Body style={main}>
		  <Container style={container}>
			<Section>
			  <Text style={text}>Hallo {`{{ user_name }}`},</Text>
			  <Text style={text}>
				Je bent uitgenodigd om deel te nemen aan de Timespace van {`{{ tenant_name }}`}, klik hieronder om je account aan te maken:
			  </Text>
			  <Button style={button} href="{{ invite_link }}">
				Uitnodiging accepteren
			  </Button>
			  <Text style={text}>
				Als je niet deel wil nemen aan deze Timespace, kun je deze e-mail gewoon negeren en verwijderen.
			  </Text>
			  <Text style={text}>
				Om je account veilig te houden, stuur deze e-mail alsjeblieft niet door naar iemand anders.
			  </Text>
			</Section>
		  </Container>
		</Body>
	  </Html>
	);
};
  
  export default TimespaceConfirmEmailEmail;
  
  const main = {
	backgroundColor: "#f6f9fc",
	padding: "10px 0",
  };
  
  const container = {
	backgroundColor: "#ffffff",
	border: "1px solid #f0f0f0",
	padding: "45px",
  };
  
  const text = {
	fontSize: "16px",
	fontFamily: `ui-sans-serif, system-ui, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"`,
	fontWeight: "300",
	color: "#404040",
	lineHeight: "26px",
  };

  const boldText = {
	fontWeight: "bold"
  }
  
  const button = {
	backgroundColor: colors.buttonPrimary,
	borderRadius: "4px",
	color: "#fff",
	fontFamily: "'Open Sans', 'Helvetica Neue', Arial",
	fontSize: "15px",
	textDecoration: "none",
	textAlign: "center" as const,
	display: "block",
	width: "210px",
	padding: "14px 7px",
  };
  
  const anchor = {
	textDecoration: "underline",
  };
  