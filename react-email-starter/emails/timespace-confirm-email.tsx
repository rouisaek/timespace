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
  
  export const DropboxResetPasswordEmail = () => {
	return (
	  <Html>
		<Head />
		<Preview>Dropbox reset your password</Preview>
		<Body style={main}>
		  <Container style={container}>
			
			<Section>
			  <Text style={text}>Hi {`{{ userFirstName }}`},</Text>
			  <Text style={text}>
				Someone recently requested an email confirmation for your Timespace
				account. If this was you, you can confirm your email by clicking the button here:
			  </Text>
			  <Button style={button} href="{{ confirmEmailLink }}">
				Reset password
			  </Button>
			  <Text style={text}>
				If you don&apos;t didn&apos;t
				request this, just ignore and delete this message.
			  </Text>
			  <Text style={text}>
				To keep your account secure, please don&apos;t forward this email
				to anyone.
			  </Text>
			</Section>
		  </Container>
		</Body>
	  </Html>
	);
  };
  
  export default DropboxResetPasswordEmail;
  
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
	fontFamily:
	  "'Open Sans', 'HelveticaNeue-Light', 'Helvetica Neue Light', 'Helvetica Neue', Helvetica, Arial, 'Lucida Grande', sans-serif",
	fontWeight: "300",
	color: "#404040",
	lineHeight: "26px",
  };
  
  const button = {
	backgroundColor: "#007ee6",
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
  