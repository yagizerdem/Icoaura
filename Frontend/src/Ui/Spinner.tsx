import React from "react";
import styled, { keyframes } from "styled-components";

interface SpinnerProps {
  size?: number | string;
  color?: string;
  borderWidth?: number;
}

const spin = keyframes`
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
`;

const Loader = styled.span<SpinnerProps>`
  display: inline-block;
  width: ${({ size }) =>
    typeof size === "number" ? `${size}px` : size || "48px"};
  height: ${({ size }) =>
    typeof size === "number" ? `${size}px` : size || "48px"};
  border: ${({ borderWidth }) => borderWidth || 5}px solid
    ${({ color }) => color || "#fff"};
  border-bottom-color: transparent;
  border-radius: 50%;
  box-sizing: border-box;
  animation: ${spin} 1s linear infinite;
`;

export const Spinner: React.FC<SpinnerProps> = ({
  size,
  color,
  borderWidth,
}) => {
  return <Loader size={size} color={color} borderWidth={borderWidth} />;
};
